using HT_Web.Models.Traceability;

namespace HT_Web.Services.Traceability;

public interface ITraceabilityService
{
    Task<List<TraceabilityRecord>> GetTraceabilityAsync(
        string searchValue,
        bool isFinalLabel);

    Task<List<SolvedErrorRecord>> GetErrorsAsync(
        string searchValue);
}