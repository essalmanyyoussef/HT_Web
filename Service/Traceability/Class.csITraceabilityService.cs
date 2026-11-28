using HT_Web.Models.Traceability;

namespace HT_Web.Services.Traceability;

public interface ITraceabilityService
{
    Task<TraceabilitySearchResult> SearchAsync(
        TraceabilitySearchRequest request);

    Task<TraceabilitySearchResult> SearchProcessHistoryAsync(
        TraceabilitySearchRequest request);

    Task<List<SolvedErrorRecord>> SearchErrorsAsync(
        TraceabilitySearchRequest request,
        List<TraceabilityRecord> processHistory,
        List<string> searchKeys);
}