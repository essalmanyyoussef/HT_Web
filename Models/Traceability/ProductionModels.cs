namespace HT_Web.Models.Traceability;

public class ProductionRawRecord
{
    public DateTime Date { get; set; }
    public int Hour { get; set; }
    public string Hostname { get; set; } = string.Empty;
    public string Bank { get; set; } = string.Empty;
    public string Ref { get; set; } = string.Empty;
    public int TotalQty { get; set; }

    public string Customer { get; set; } = string.Empty;
    public string Project { get; set; } = string.Empty;
    public string Family { get; set; } = string.Empty;
    public string Line { get; set; } = "L1";
}

public class ProductionReferenceSummary
{
    public string Ref { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string Project { get; set; } = string.Empty;
    public string Family { get; set; } = string.Empty;
    public int TotalQty { get; set; }
    public double SharePercent { get; set; }
}

public class ProductionBucketItem
{
    public string Label { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public int Qty { get; set; }
    public double SharePercent { get; set; }
    public double AvgPerHour { get; set; }
}