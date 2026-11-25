namespace HT_Web.Models.Traceability;

public class HostnameLookupItem
{
    public string Hostname { get; set; } = string.Empty;
    public string Station { get; set; } = string.Empty;
    public string Bank { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string Project { get; set; } = string.Empty;
    public string Family { get; set; } = string.Empty;
    public string Line { get; set; } = string.Empty;
}

public class StationReferenceSummary
{
    public string Reference { get; set; } = string.Empty;
    public int TotalQty { get; set; }
    public int ReworkedQty { get; set; }
    public double SharePercent { get; set; }
}