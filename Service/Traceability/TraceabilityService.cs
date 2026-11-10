using HT_Web.Models.Traceability;
using HT_Web.Repositories;

namespace HT_Web.Services.Traceability;

public interface ITraceabilityService
{
    Task<List<TraceabilityRecord>> GetTraceabilityAsync(
        string searchValue,
        bool isFinalLabel);

    Task<List<SolvedErrorRecord>> GetErrorsAsync(
        string searchValue);
}

public class TraceabilityService : ITraceabilityService
{
    private readonly ITraceabilityRepository _repository;

    public TraceabilityService(
        ITraceabilityRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TraceabilityRecord>>
        GetTraceabilityAsync(
        string searchValue,
        bool isFinalLabel)
    {
        if (isFinalLabel)
        {
            return await _repository
                .GetTraceabilityByFinalLabelAsync(searchValue);
        }

        return await _repository
            .GetTraceabilityByTravelTicketAsync(searchValue);
    }

    public async Task<List<SolvedErrorRecord>>
        GetErrorsAsync(
        string searchValue)
    {
        return await _repository
            .GetErrorsAsync(searchValue);
    }
}