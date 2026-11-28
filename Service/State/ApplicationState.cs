using HT_Web.Models.Authentication;
using HT_Web.Models.Traceability;

namespace HT_Web.Services.State;

public class ApplicationState
{
    public UserContext? CurrentUser { get; private set; }
    public bool IsCheckingAccess { get; private set; } = true;
    public string AccessStatusMessage { get; private set; } = "Resolving Active Directory identity...";

    // In-memory tab-session state (persists while navigating pages, clears when tab closes)
    public TraceabilityPageState Traceability { get; } = new();
    public ProductionPageState Production { get; } = new();
    public StationHistoryPageState StationHistory { get; } = new();
    public ReportsPageState Reports { get; } = new();

    public event Action? OnChange;

    public void SetCheckingAccess(bool isChecking, string? message = null)
    {
        IsCheckingAccess = isChecking;
        if (!string.IsNullOrWhiteSpace(message))
        {
            AccessStatusMessage = message;
        }
        NotifyStateChanged();
    }

    public void SetCurrentUser(UserContext user)
    {
        CurrentUser = user;
        IsCheckingAccess = false;
        NotifyStateChanged();
    }

    public void SetSelectedSite(string site)
    {
        if (CurrentUser == null)
            return;

        CurrentUser.SelectedSite = site;
        NotifyStateChanged();
    }

    private void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}

public class TraceabilityPageState
{
    public string SearchValue { get; set; } = string.Empty;
    public bool IsFinalLabel { get; set; } = true;
    public bool IncludeAllErrors { get; set; } = true;
    public bool HasSearched { get; set; } = false;
    public List<TraceabilityRecord> ProcessHistory { get; set; } = new();
    public List<SolvedErrorRecord> Errors { get; set; } = new();
    public HarnessInformation? HarnessInfo { get; set; }
}

public class ProductionPageState
{
    public string SelectedSite { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; } = DateTime.Today;
    public DateTime? EndDate { get; set; } = DateTime.Today;
    public string ActivePreset { get; set; } = "TODAY";
    public string SelectedShift { get; set; } = "ALL";
    public string SelectedCustomer { get; set; } = "ALL";
    public string SelectedProject { get; set; } = "ALL";
    public string SelectedFamily { get; set; } = "ALL";
    public string SelectedLine { get; set; } = "ALL";
    public bool HasLoaded { get; set; } = false;
    public List<ProductionRawRecord> RawRows { get; set; } = new();
}

public class StationHistoryPageState
{
    public string Hostname { get; set; } = string.Empty;
    public string ResolvedSite { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; } = DateTime.Today;
    public DateTime? EndDate { get; set; } = DateTime.Today;
    public string ActivePreset { get; set; } = "TODAY";
    public bool HasSearched { get; set; } = false;
    public List<TraceabilityRecord> Records { get; set; } = new();
}

public class ReportsPageState
{
    public DateTime? StartDate { get; set; } = DateTime.Today.AddDays(-13);
    public DateTime? EndDate { get; set; } = DateTime.Today;
    public string ActivePreset { get; set; } = "14D";
    public string SelectedSite { get; set; } = "ALL";
    public bool HasLoaded { get; set; } = false;
    public TrackingStatisticsResult Stats { get; set; } = new();
}

public class SiteShiftSettings
{
    public int S1Start { get; set; } = 6;
    public int S1End { get; set; } = 14;
    public int S2Start { get; set; } = 14;
    public int S2End { get; set; } = 22;
    public int S3Start { get; set; } = 22;
    public int S3End { get; set; } = 6;
}