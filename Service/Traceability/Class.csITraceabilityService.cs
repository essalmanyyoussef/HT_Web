using HT_Web.Models.Traceability;

namespace HT_Web.Services.Traceability;

public interface ITraceabilityService
{
    Task<TraceabilitySearchResult> SearchAsync(
        TraceabilitySearchRequest request);
}
