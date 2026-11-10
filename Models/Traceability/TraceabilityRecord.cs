namespace HT_Web.Models.Traceability;

public class TraceabilityRecord
{
    public string Reference { get; set; } = "";

    public string TravelTicketLabel { get; set; } = "";

    public string Station { get; set; } = "";

    public string SecondLabel { get; set; } = "";

    public string ThirdTravelTicketLabel { get; set; } = "";

    public string FourthTravelTicketLabel { get; set; } = "";

    public string FinalLabel { get; set; } = "";

    public DateTime TestDate { get; set; }

    public string Reworked { get; set; } = "";

    public string ReworkId { get; set; } = "";

    public string Hostname { get; set; } = "";
}