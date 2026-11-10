using HT_Web.Models.Authentication;

namespace HT_Web.Services.Authentication;

public class MockUserAccessService : IUserAccessService
{
    public Task<UserContext?> GetCurrentUserAsync()
    {
        var user = new UserContext
        {
            DisplayName = "Essalmany, Youssef (Kenitra)",
            Email = "youssef.essalmany@sews-e.com",
            AccessibleSites = new List<string>
            {
                "MFZ"
            },
            SelectedSite = "MFZ",
            IsAdmin = true
        };

        return Task.FromResult<UserContext?>(user);
    }
}