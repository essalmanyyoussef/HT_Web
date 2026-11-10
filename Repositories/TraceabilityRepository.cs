using HT_Web.Models.Traceability;

namespace HT_Web.Repositories;

public class TraceabilityRepository : ITraceabilityRepository
{
    public Task<List<TraceabilityRecord>> GetTraceabilityByFinalLabelAsync(
        string finalLabel)
    {
        return Task.FromResult(new List<TraceabilityRecord>());
    }

    public Task<List<TraceabilityRecord>> GetTraceabilityByTravelTicketAsync(
        string travelTicket)
    {
        return Task.FromResult(new List<TraceabilityRecord>());
    }

    public Task<List<SolvedErrorRecord>> GetErrorsAsync(
        string travelTicket)
    {
        return Task.FromResult(new List<SolvedErrorRecord>());
    }
}