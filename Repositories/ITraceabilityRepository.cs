using HT_Web.Models.Traceability;

namespace HT_Web.Repositories;

public interface ITraceabilityRepository
{
    Task<List<TraceabilityRecord>> GetTraceabilityByFinalLabelAsync(
        string finalLabel);

    Task<List<TraceabilityRecord>> GetTraceabilityByTravelTicketAsync(
        string travelTicket);

    Task<List<SolvedErrorRecord>> GetErrorsAsync(
        string travelTicket);
}