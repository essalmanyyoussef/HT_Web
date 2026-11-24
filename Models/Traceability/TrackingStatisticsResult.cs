namespace HT_Web.Models.Traceability;

public class TrackingStatisticsResult
{
    public int TotalSearches { get; set; }
    public int DistinctUsersCount { get; set; }
    public List<DailyTrackingStat> DailyStats { get; set; } = new();
    public List<SiteTrackingStat> SiteStats { get; set; } = new();
    public List<HourlyTrackingStat> HourlyStats { get; set; } = new();
}

public class DailyTrackingStat
{
    public DateTime Date { get; set; }
    public int TotalCount { get; set; }
    public int DistinctUsersCount { get; set; }
}

public class SiteTrackingStat
{
    public string Site { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public int DistinctUsersCount { get; set; }
    public double SharePercent { get; set; }
    public double AvgSearchesPerUser => DistinctUsersCount > 0
        ? Math.Round((double)TotalCount / DistinctUsersCount, 1)
        : 0;
}

public class HourlyTrackingStat
{
    public int Hour { get; set; }
    public int TotalCount { get; set; }
}