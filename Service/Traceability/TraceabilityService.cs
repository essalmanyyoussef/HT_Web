using HT_Web.Models.Traceability;
using HT_Web.Repositories;
using Microsoft.Extensions.Logging;

namespace HT_Web.Services.Traceability;

public class TraceabilityService : ITraceabilityService
{
    private readonly ITraceabilityRepository _repository;
    private readonly ILogger<TraceabilityService> _logger;

    public TraceabilityService(
        ITraceabilityRepository repository,
        ILogger<TraceabilityService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<TraceabilitySearchResult> SearchAsync(TraceabilitySearchRequest request)
    {
        var result = await SearchProcessHistoryAsync(request);
        result.Errors = await SearchErrorsAsync(request, result.ProcessHistory, result.ErrorSearchKeys);
        return result;
    }

    public async Task<TraceabilitySearchResult> SearchProcessHistoryAsync(TraceabilitySearchRequest request)
    {
        var result = new TraceabilitySearchResult();
        var searchKeys = new List<string>();

        if (request.IsFinalLabel)
        {
            var initial = await _repository.GetTraceabilityByFinalLabelAsync(
                request.Site,
                request.SearchValue);

            if (initial.Count > 0)
            {
                var first = initial[0];

                if (!string.IsNullOrWhiteSpace(first.TravelTicketLabel))
                    searchKeys.Add(first.TravelTicketLabel);
                if (!string.IsNullOrWhiteSpace(first.SecondLabel))
                    searchKeys.Add(first.SecondLabel);
                if (!string.IsNullOrWhiteSpace(first.ThirdTravelTicketLabel))
                    searchKeys.Add(first.ThirdTravelTicketLabel);
                if (!string.IsNullOrWhiteSpace(first.FourthTravelTicketLabel))
                    searchKeys.Add(first.FourthTravelTicketLabel);

                if (searchKeys.Count > 0)
                {
                    result.ProcessHistory = await _repository.GetTraceabilityByTravelTicketAsync(
                        request.Site,
                        searchKeys[0]);

                    if (result.ProcessHistory.Count > 15)
                    {
                        result.ProcessHistory = initial;
                    }
                }
                else
                {
                    result.ProcessHistory = initial;
                }
            }
        }
        else
        {
            result.ProcessHistory = await _repository.GetTraceabilityByTravelTicketAsync(
                request.Site,
                request.SearchValue);

            searchKeys.Add(request.SearchValue);
        }

        var distinctEtTargets = ExtractDistinctEtTargets(result.ProcessHistory);
        result.HarnessInformation = await ResolveHarnessInformationAsync(
            request.Site,
            request.SearchValue,
            distinctEtTargets);

        result.ProcessHistory = GroupProcessHistory(result.ProcessHistory);
        result.ErrorSearchKeys = searchKeys;

        return result;
    }

    public async Task<List<SolvedErrorRecord>> SearchErrorsAsync(
        TraceabilitySearchRequest request,
        List<TraceabilityRecord> processHistory,
        List<string> searchKeys)
    {
        var distinctEtTargets = ExtractDistinctEtTargets(processHistory);
        List<SolvedErrorRecord> rawErrors;

        if (request.IncludeAllErrors && distinctEtTargets.Count > 0)
        {
            // Run ONLY the targeted ET date-window query (no redundant 900M-row fallback scan on PASS harnesses)
            rawErrors = await GetEtDateRangeErrorsAsync(request.Site, distinctEtTargets);
        }
        else
        {
            // Single-scan WHERE Part IN @Keys instead of N sequential full table scans
            rawErrors = await _repository.GetErrorsByKeysAsync(request.Site, searchKeys);
        }

        var groupedErrors = GroupErrorRecords(rawErrors);

        try
        {
            var searchType = request.IsFinalLabel ? "Final Label" : "Travel Ticket";
            var user = string.IsNullOrWhiteSpace(request.Username) ? "Unknown" : request.Username;
            var info = $"Steps: {processHistory.Count}, Errors: {groupedErrors.Count} - HT_Web";

            await _repository.LogTrackingAsync(
                request.SearchValue,
                request.Site,
                user,
                DateTime.Now,
                info,
                searchType);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to write tracking log for {SearchValue}", request.SearchValue);
        }

        return groupedErrors;
    }

    private static List<(string Host, DateTime TestDate)> ExtractDistinctEtTargets(List<TraceabilityRecord> history)
    {
        return history
            .Where(r => IsEtStation(r.Station) && !string.IsNullOrWhiteSpace(r.Hostname))
            .SelectMany(r => r.Hostname
                .Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(h => (Host: h, r.TestDate)))
            .Distinct()
            .ToList();
    }

    private async Task<List<SolvedErrorRecord>> GetEtDateRangeErrorsAsync(
        string site,
        List<(string Host, DateTime TestDate)> etTargets)
    {
        if (etTargets.Count == 0)
            return new List<SolvedErrorRecord>();

        var tasks = etTargets.Select(async target =>
        {
            var dates = await _repository.GetTestDatesAsync(site, target.Host, target.TestDate);
            if (dates is null)
                return new List<SolvedErrorRecord>();

            if ((target.TestDate - dates.Value.DateBegin).TotalHours > 5)
                return new List<SolvedErrorRecord>();

            return await _repository.GetErrorsByDateAsync(
                site,
                target.Host,
                dates.Value.DateBegin,
                dates.Value.DateEnd);
        });

        var results = await Task.WhenAll(tasks);
        return results.SelectMany(x => x).ToList();
    }

    private async Task<HarnessInformation?> ResolveHarnessInformationAsync(
        string site,
        string serialNumber,
        List<(string Host, DateTime TestDate)> etTargets)
    {
        if (etTargets.Count == 0)
            return null;

        var bankTasks = etTargets.Select(t => _repository.GetStationFamilyAsync(site, t.Host, t.TestDate));
        var banks = await Task.WhenAll(bankTasks);

        foreach (var bank in banks)
        {
            if (string.IsNullOrWhiteSpace(bank))
                continue;

            var parts = bank.Split('_', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 5
                && parts[^2].StartsWith("L", StringComparison.OrdinalIgnoreCase)
                && (parts[^1].Equals("ET", StringComparison.OrdinalIgnoreCase) || parts[^1].Equals("CT", StringComparison.OrdinalIgnoreCase)))
            {
                var customerCode = parts[0].ToUpperInvariant();
                var customer = customerCode switch
                {
                    "PSA" or "STL" => "STELLANTIS",
                    "TYT" or "TOY" => "TOYOTA",
                    "SUZ" => "SUZUKI",
                    "RNLT" or "RENA" or "RSA" or "RNL" or "RNT" => "RENAULT",
                    "NIS" or "NISS" or "NSN" => "NISSAN",
                    _ => customerCode
                };

                return new HarnessInformation
                {
                    SerialNumber = serialNumber,
                    Customer = customer,
                    Project = parts[1],
                    Family = string.Join("_", parts.Skip(2).Take(parts.Length - 4)),
                    Line = parts[^2]
                };
            }
        }

        return null;
    }

    private static List<TraceabilityRecord> GroupProcessHistory(List<TraceabilityRecord> records)
    {
        if (records.Count <= 1)
            return records;

        return records
            .GroupBy(r => new
            {
                Reference = r.Reference ?? string.Empty,
                TravelTicketLabel = r.TravelTicketLabel ?? string.Empty,
                Station = r.Station ?? string.Empty,
                SecondLabel = r.SecondLabel ?? string.Empty,
                ThirdTravelTicketLabel = r.ThirdTravelTicketLabel ?? string.Empty,
                FourthTravelTicketLabel = r.FourthTravelTicketLabel ?? string.Empty,
                FinalLabel = r.FinalLabel ?? string.Empty,
                r.TestDate,
                Reworked = r.Reworked ?? string.Empty,
                ReworkId = r.ReworkId ?? string.Empty
            })
            .Select(g =>
            {
                var first = g.First();
                var hostnames = g
                    .Select(x => x.Hostname?.Trim())
                    .Where(h => !string.IsNullOrWhiteSpace(h))
                    .Distinct(StringComparer.OrdinalIgnoreCase);

                return new TraceabilityRecord
                {
                    Reference = first.Reference,
                    TravelTicketLabel = first.TravelTicketLabel,
                    Station = first.Station,
                    SecondLabel = first.SecondLabel,
                    ThirdTravelTicketLabel = first.ThirdTravelTicketLabel,
                    FourthTravelTicketLabel = first.FourthTravelTicketLabel,
                    FinalLabel = first.FinalLabel,
                    TestDate = first.TestDate,
                    Reworked = first.Reworked,
                    ReworkId = first.ReworkId,
                    Hostname = string.Join(" / ", hostnames)
                };
            })
            .ToList();
    }

    private static List<SolvedErrorRecord> GroupErrorRecords(List<SolvedErrorRecord> errors)
    {
        if (errors.Count <= 1)
            return errors;

        return errors
            .GroupBy(e => new
            {
                Reference = e.Reference ?? string.Empty,
                Batch = e.Batch ?? string.Empty,
                TravelTicketLabel = e.TravelTicketLabel ?? string.Empty,
                WorkerId = e.WorkerId ?? string.Empty,
                e.ErrorType,
                Tmp01 = e.Tmp01 ?? string.Empty,
                Tmp02 = e.Tmp02 ?? string.Empty,
                Object = e.Object ?? string.Empty,
                Wire1 = e.Wire1 ?? string.Empty,
                Wire2 = e.Wire2 ?? string.Empty,
                Way = e.Way ?? string.Empty,
                e.StartDate,
                e.EndDate,
                e.TimeU,
                CPV1 = e.CPV1 ?? string.Empty,
                CPV2 = e.CPV2 ?? string.Empty,
                CPV3 = e.CPV3 ?? string.Empty,
                CPV4 = e.CPV4 ?? string.Empty
            })
            .Select(g =>
            {
                var first = g.First();
                var hostnames = g
                    .Select(x => x.Hostname?.Trim())
                    .Where(h => !string.IsNullOrWhiteSpace(h))
                    .Distinct(StringComparer.OrdinalIgnoreCase);

                return new SolvedErrorRecord
                {
                    Reference = first.Reference,
                    Batch = first.Batch,
                    TravelTicketLabel = first.TravelTicketLabel,
                    WorkerId = first.WorkerId,
                    ErrorType = first.ErrorType,
                    Tmp01 = first.Tmp01,
                    Tmp02 = first.Tmp02,
                    Object = first.Object,
                    Wire1 = first.Wire1,
                    Wire2 = first.Wire2,
                    Way = first.Way,
                    StartDate = first.StartDate,
                    EndDate = first.EndDate,
                    TimeU = first.TimeU,
                    CPV1 = first.CPV1,
                    CPV2 = first.CPV2,
                    CPV3 = first.CPV3,
                    CPV4 = first.CPV4,
                    Hostname = string.Join(" / ", hostnames)
                };
            })
            .ToList();
    }

    private static bool IsEtStation(string? station)
    {
        if (string.IsNullOrWhiteSpace(station))
            return false;

        return station.Contains("ET", StringComparison.OrdinalIgnoreCase)
            || station.Contains("ELEC", StringComparison.OrdinalIgnoreCase)
            || station.Contains("ELECTRIC", StringComparison.OrdinalIgnoreCase)
            || station.Contains("Test Electric", StringComparison.OrdinalIgnoreCase);
    }
}