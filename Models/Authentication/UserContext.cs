namespace HT_Web.Models.Authentication;

public class UserContext
{
    public string DisplayName { get; set; } = "";

    public string Email { get; set; } = "";

    public List<string> AccessibleSites { get; set; } = new();

    public string SelectedSite { get; set; } = "";

    public bool IsAdmin { get; set; }
}