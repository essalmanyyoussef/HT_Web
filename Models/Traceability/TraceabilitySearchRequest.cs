namespace HT_Web.Models.Traceability;

public class TraceabilitySearchRequest
{
    public string SearchValue { get; set; } = string.Empty;

    public bool IsFinalLabel { get; set; } = true;

    public bool IncludeAllErrors { get; set; } = true;

    public string Site { get; set; } = string.Empty;

    public string? Username { get; set; }
}