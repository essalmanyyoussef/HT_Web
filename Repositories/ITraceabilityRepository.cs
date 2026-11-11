using HT_Web.Models.Traceability;

namespace HT_Web.Repositories;

public interface ITraceabilityRepository
{
    Task<List<TraceabilityRecord>>
        GetTraceabilityByFinalLabelAsync(
            string site,
            string finalLabel);

    Task<List<TraceabilityRecord>>
        GetTraceabilityByTravelTicketAsync(
            string site,
            string travelTicket);

    Task<List<SolvedErrorRecord>>
        GetErrorsAsync(
            string site,
            string travelTicket);

    Task<List<ErrorSearchResult>>
    GetEtStationsAsync(
        string site,
        string travelTicket);

}