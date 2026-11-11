namespace HT_Web.Models.Traceability;

public class TraceabilitySearchResult
{
    public HarnessInformation? HarnessInformation { get; set; }

    public List<TraceabilityRecord> ProcessHistory { get; set; } = new();

    public List<SolvedErrorRecord> Errors { get; set; } = new();

    public List<string> ErrorSearchKeys { get; set; } = new();
}