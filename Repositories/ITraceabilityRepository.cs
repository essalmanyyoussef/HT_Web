using HT_Web.Models.Traceability;

namespace HT_Web.Repositories;

public interface ITraceabilityRepository
{
    Task<List<TraceabilityRecord>> GetTraceabilityByFinalLabelAsync(
        string site,
        string finalLabel);

    Task<List<TraceabilityRecord>> GetTraceabilityByTravelTicketAsync(
        string site,
        string travelTicket);

    Task<List<SolvedErrorRecord>> GetErrorsAsync(
        string site,
        string travelTicket);

    Task<List<ErrorSearchResult>> GetEtStationsAsync(
        string site,
        string travelTicket);

    Task<string?> GetStationFamilyAsync(
        string site,
        string hostname,
        DateTime testDate);

    Task<(DateTime DateBegin, DateTime DateEnd)?> GetTestDatesAsync(
        string site,
        string hostname,
        DateTime testDate);

    Task<List<SolvedErrorRecord>> GetErrorsByDateAsync(
        string site,
        string hostname,
        DateTime dateBegin,
        DateTime dateEnd);
}
