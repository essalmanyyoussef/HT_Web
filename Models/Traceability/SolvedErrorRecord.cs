namespace HT_Web.Models.Traceability;

public class SolvedErrorRecord
{
    public string Reference { get; set; } = "";

    public string Batch { get; set; } = "";

    public string TravelTicketLabel { get; set; } = "";

    public string WorkerId { get; set; } = "";

    public int ErrorType { get; set; }

    public string Tmp01 { get; set; } = "";

    public string Tmp02 { get; set; } = "";

    public string Object { get; set; } = "";

    public string Wire1 { get; set; } = "";

    public string Wire2 { get; set; } = "";

    public string Way { get; set; } = "";

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int TimeU { get; set; }

    public string CPV1 { get; set; } = "";

    public string CPV2 { get; set; } = "";

    public string CPV3 { get; set; } = "";

    public string CPV4 { get; set; } = "";

    public string Hostname { get; set; } = "";
}