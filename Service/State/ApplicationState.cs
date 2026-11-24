using HT_Web.Models.Authentication;

namespace HT_Web.Services.State;

public class ApplicationState
{
    public UserContext? CurrentUser { get; private set; }
    public bool IsCheckingAccess { get; private set; } = true;
    public string AccessStatusMessage { get; private set; } = "Resolving Active Directory identity...";

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