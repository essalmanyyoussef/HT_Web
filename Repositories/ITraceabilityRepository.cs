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

    Task LogTrackingAsync(
        string serialNumber,
        string site,
        string username,
        DateTime timestamp,
        string additionalInfo,
        string type);

    Task<TrackingStatisticsResult> GetTrackingStatisticsAsync(
        DateTime startDate,
        DateTime endDate,
        string? siteFilter,
        List<string> allowedSites);
    Task<List<ProductionRawRecord>> GetProductionDataAsync(
        string site,
        DateTime startDate,
        DateTime endDate);
    Task<List<TraceabilityRecord>> GetStationHistoryAsync(
        string site,
        string hostname,
        DateTime startDate,
        DateTime endDate);

    Task<List<HostnameLookupItem>> FindHostnamesByBarcodeAsync(
        string site,
        string searchMode,
        string searchValue);

    Task<List<HostnameLookupItem>> FindHostnamesBySiteBanksAsync(
        string site);
    Task<List<SolvedErrorRecord>> GetErrorsByKeysAsync(
        string site,
        List<string> travelTickets,
        DateTime? minDate = null,
        DateTime? maxDate = null);
}