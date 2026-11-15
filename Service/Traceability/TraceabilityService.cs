using HT_Web.Models.Traceability;
using HT_Web.Repositories;

namespace HT_Web.Services.Traceability;

public class TraceabilityService : ITraceabilityService
{
    private readonly ITraceabilityRepository _repository;

    public TraceabilityService(ITraceabilityRepository repository)
    {
        _repository = repository;
    }

    public async Task<TraceabilitySearchResult> SearchAsync(TraceabilitySearchRequest request)
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

        // 1. Resolve Customer / Project / Family / Line (matching Form1.GetHarnessFamilyInformation)
        result.HarnessInformation = await ResolveHarnessInformationAsync(
            request.Site,
            request.SearchValue,
            result.ProcessHistory);

        // 2. Resolve Errors (matching Form1 ET_Errors date-range vs SelvedError direct lookup)
        if (request.IncludeAllErrors)
        {
            result.Errors = await GetEtDateRangeErrorsAsync(request.Site, result.ProcessHistory);

            // Fallback to direct key lookup if ET date-range returned nothing
            if (result.Errors.Count == 0)
            {
                result.Errors = await GetDirectErrorsByKeysAsync(request.Site, searchKeys);
            }
        }
        else
        {
            result.Errors = await GetDirectErrorsByKeysAsync(request.Site, searchKeys);
        }

        result.ErrorSearchKeys = searchKeys;
        return result;
    }

    private async Task<List<SolvedErrorRecord>> GetDirectErrorsByKeysAsync(string site, List<string> searchKeys)
    {
        foreach (var key in searchKeys.Distinct())
        {
            var errors = await _repository.GetErrorsAsync(site, key);
            if (errors.Count > 0)
            {
                return errors;
            }
        }
        return new List<SolvedErrorRecord>();
    }

    private async Task<List<SolvedErrorRecord>> GetEtDateRangeErrorsAsync(string site, List<TraceabilityRecord> history)
    {
        var allErrors = new List<SolvedErrorRecord>();

        foreach (var row in history.Where(r => IsEtStation(r.Station) && !string.IsNullOrWhiteSpace(r.Hostname)))
        {
            var host = row.Hostname.Split('/')[0].Trim();
            var dates = await _repository.GetTestDatesAsync(site, host, row.TestDate);
            if (dates is null)
                continue;

            if ((row.TestDate - dates.Value.DateBegin).TotalHours > 5)
                continue;

            var errors = await _repository.GetErrorsByDateAsync(
                site,
                host,
                dates.Value.DateBegin,
                dates.Value.DateEnd);

            if (errors.Count > 0)
            {
                allErrors.AddRange(errors);
            }
        }

        return allErrors;
    }

    private async Task<HarnessInformation?> ResolveHarnessInformationAsync(
        string site,
        string serialNumber,
        List<TraceabilityRecord> history)
    {
        foreach (var row in history.Where(r => IsEtStation(r.Station) && !string.IsNullOrWhiteSpace(r.Hostname)))
        {
            var host = row.Hostname.Split('/')[0].Trim();
            var bank = await _repository.GetStationFamilyAsync(site, host, row.TestDate);
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
