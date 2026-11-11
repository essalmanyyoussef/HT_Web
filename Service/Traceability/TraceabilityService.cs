using HT_Web.Models.Traceability;
using HT_Web.Repositories;

namespace HT_Web.Services.Traceability;

public interface ITraceabilityService
{
    Task<TraceabilitySearchResult> SearchAsync(
        TraceabilitySearchRequest request);
}

public class TraceabilityService : ITraceabilityService
{
    private readonly ITraceabilityRepository _repository;

    public TraceabilityService(
        ITraceabilityRepository repository)
    {
        _repository = repository;
    }

    public async Task<TraceabilitySearchResult> SearchAsync(
    TraceabilitySearchRequest request)
    {
        List<TraceabilityRecord> processHistory;

        var searchKeys = new List<string>();

        if (request.IsFinalLabel)
        {
            var firstResult =
                await _repository.GetTraceabilityByFinalLabelAsync(
                    request.Site,
                    request.SearchValue);

            if (!firstResult.Any())
            {
                return new TraceabilitySearchResult();
            }

            var firstRow = firstResult.First();

            if (!string.IsNullOrWhiteSpace(firstRow.TravelTicketLabel))
                searchKeys.Add(firstRow.TravelTicketLabel);

            if (!string.IsNullOrWhiteSpace(firstRow.SecondLabel))
                searchKeys.Add(firstRow.SecondLabel);

            if (!string.IsNullOrWhiteSpace(firstRow.ThirdTravelTicketLabel))
                searchKeys.Add(firstRow.ThirdTravelTicketLabel);

            if (!string.IsNullOrWhiteSpace(firstRow.FourthTravelTicketLabel))
                searchKeys.Add(firstRow.FourthTravelTicketLabel);

            processHistory =
                await _repository.GetTraceabilityByTravelTicketAsync(
                    request.Site,
                    firstRow.TravelTicketLabel);
        }
        else
        {
            processHistory =
                await _repository.GetTraceabilityByTravelTicketAsync(
                    request.Site,
                    request.SearchValue);

            searchKeys.Add(request.SearchValue);
        }

        List<SolvedErrorRecord> errors = new();

        foreach (var key in searchKeys)
        {
            errors =
                await _repository.GetErrorsAsync(
                    request.Site,
                    key);

            if (errors.Any())
                break;
        }

        return new TraceabilitySearchResult
        {
            ProcessHistory = processHistory,
            Errors = errors,
            ErrorSearchKeys = searchKeys
        };
    }



}