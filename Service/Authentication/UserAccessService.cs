using System.DirectoryServices.AccountManagement;
using System.Runtime.Versioning;
using System.Security.Principal;
using HT_Web.Models.Authentication;
using Microsoft.AspNetCore.Http;

namespace HT_Web.Services.Authentication;

public class UserAccessService : IUserAccessService
{
    private readonly AuthenticationApiService _authApi;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserAccessService(
        AuthenticationApiService authApi,
        IHttpContextAccessor httpContextAccessor)
    {
        _authApi = authApi;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<UserContext?> GetCurrentUserAsync()
    {
        var (displayName, userName, email) = ResolveCurrentUser();

        var authResponse = await _authApi.GetSitesAsync(displayName);
        if (!authResponse.HasAccess && !string.Equals(userName, displayName, StringComparison.OrdinalIgnoreCase))
        {
            authResponse = await _authApi.GetSitesAsync(userName);
        }

        return new UserContext
        {
            DisplayName = displayName,
            Email = email,
            AccessibleSites = authResponse.AccessibleSites,
            SelectedSite = authResponse.AccessibleSites.FirstOrDefault() ?? string.Empty,
            IsAdmin = authResponse.HasAccess
        };
    }

    private (string DisplayName, string UserName, string Email) ResolveCurrentUser()
    {
        // 1. Try HTTP Context Windows/Authenticated identity first (IIS / Intranet)
        var identityName = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        var shortName = StripDomain(identityName);

        if (OperatingSystem.IsWindows())
        {
            try
            {
                return ResolveWindowsPrincipal(shortName);
            }
            catch
            {
                // Fallback if AD / PrincipalServer is unreachable
            }
        }

        var fallbackUser = !string.IsNullOrWhiteSpace(shortName)
            ? shortName
            : Environment.UserName;

        return (fallbackUser, fallbackUser, $"{fallbackUser}@sews-e.com");
    }

    [SupportedOSPlatform("windows")]
    private static (string DisplayName, string UserName, string Email) ResolveWindowsPrincipal(string? fallbackName)
    {
        var principal = UserPrincipal.Current;
        if (principal != null)
        {
            var displayName = !string.IsNullOrWhiteSpace(principal.DisplayName)
                ? principal.DisplayName
                : (!string.IsNullOrWhiteSpace(fallbackName) ? fallbackName : Environment.UserName);
            var sam = !string.IsNullOrWhiteSpace(principal.SamAccountName)
                ? principal.SamAccountName
                : Environment.UserName;
            var email = !string.IsNullOrWhiteSpace(principal.EmailAddress)
                ? principal.EmailAddress
                : $"{sam}@sews-e.com";

            return (displayName, sam, email);
        }

        var winName = StripDomain(WindowsIdentity.GetCurrent()?.Name) ?? Environment.UserName;
        return (winName, winName, $"{winName}@sews-e.com");
    }

    private static string? StripDomain(string? accountName)
    {
        if (string.IsNullOrWhiteSpace(accountName))
            return null;

        var idx = accountName.IndexOf('\\');
        return idx >= 0 && idx < accountName.Length - 1
            ? accountName[(idx + 1)..]
            : accountName;
    }
}
