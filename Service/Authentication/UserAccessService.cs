using System.DirectoryServices.AccountManagement;
using System.Runtime.Versioning;
using System.Security.Principal;
using HT_Web.Models.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HT_Web.Services.Authentication;

public class UserAccessService : IUserAccessService
{
    private readonly AuthenticationApiService _authApi;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;
    private readonly ILogger<UserAccessService> _logger;

    public UserAccessService(
        AuthenticationApiService authApi,
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment env,
        IConfiguration config,
        ILogger<UserAccessService> logger)
    {
        _authApi = authApi;
        _httpContextAccessor = httpContextAccessor;
        _env = env;
        _config = config;
        _logger = logger;
    }

    public async Task<UserContext?> GetCurrentUserAsync()
    {
        var (candidates, displayName, email) = ResolveCurrentUser();

        AuthenticationResponse authResponse = new();
        string matchedName = displayName;

        foreach (var candidate in candidates)
        {
            authResponse = await _authApi.GetSitesAsync(candidate);
            if (authResponse.HasAccess)
            {
                matchedName = candidate;
                break;
            }
        }

        return new UserContext
        {
            DisplayName = matchedName,
            Email = email,
            AccessibleSites = authResponse.AccessibleSites,
            SelectedSite = authResponse.AccessibleSites.FirstOrDefault() ?? string.Empty,
            IsAdmin = authResponse.HasAccess
        };
    }

    private (List<string> Candidates, string DisplayName, string Email) ResolveCurrentUser()
    {
        var identityName = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        var samName = StripDomain(identityName)
            ?? (OperatingSystem.IsWindows() ? StripDomain(GetWindowsIdentityName()) : null)
            ?? Environment.UserName;

        if (OperatingSystem.IsWindows())
        {
            var adResult = ResolveFromActiveDirectory(samName);
            if (adResult.Candidates.Count > 1 || !string.Equals(adResult.DisplayName, samName, StringComparison.OrdinalIgnoreCase))
            {
                return adResult;
            }
        }

        var fallbackCandidates = new List<string> { samName };
        var fallbackDisplay = samName;

        if (_env.IsDevelopment())
        {
            var devUser = _config["Authentication:DevFallbackFullName"];
            if (!string.IsNullOrWhiteSpace(devUser))
            {
                fallbackCandidates.Insert(0, devUser.Trim());
                fallbackDisplay = devUser.Trim();
            }
        }

        return (fallbackCandidates, fallbackDisplay, $"{samName}@sews-e.com");
    }

    [SupportedOSPlatform("windows")]
    private static string? GetWindowsIdentityName() => WindowsIdentity.GetCurrent()?.Name;

    [SupportedOSPlatform("windows")]
    private (List<string> Candidates, string DisplayName, string Email) ResolveFromActiveDirectory(string samName)
    {
        var candidates = new List<string>();
        string displayName = samName;
        string email = $"{samName}@sews-e.com";

        try
        {
            UserPrincipal? principal = null;

            // 1. Try UserPrincipal.Current first (same as WinForms Form2)
            try
            {
                principal = UserPrincipal.Current;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "UserPrincipal.Current failed for {SamName}", samName);
            }

            // 2. Fallback to Domain PrincipalContext lookup
            if (principal == null || string.IsNullOrWhiteSpace(principal.DisplayName))
            {
                try
                {
                    using var domainContext = new PrincipalContext(ContextType.Domain);
                    principal = UserPrincipal.FindByIdentity(domainContext, IdentityType.SamAccountName, samName);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Domain PrincipalContext lookup failed for {SamName}", samName);
                }
            }

            if (principal != null)
            {
                if (!string.IsNullOrWhiteSpace(principal.DisplayName))
                {
                    candidates.Add(principal.DisplayName.Trim());
                    displayName = principal.DisplayName.Trim();
                }

                if (!string.IsNullOrWhiteSpace(principal.Name) && !candidates.Contains(principal.Name.Trim(), StringComparer.OrdinalIgnoreCase))
                {
                    candidates.Add(principal.Name.Trim());
                }

                var fullName = $"{principal.GivenName} {principal.Surname}".Trim();
                if (!string.IsNullOrWhiteSpace(fullName) && !candidates.Contains(fullName, StringComparer.OrdinalIgnoreCase))
                {
                    candidates.Add(fullName);
                }

                if (!string.IsNullOrWhiteSpace(principal.EmailAddress))
                {
                    email = principal.EmailAddress.Trim();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Active Directory resolution failed for {SamName}", samName);
        }

        if (!candidates.Contains(samName, StringComparer.OrdinalIgnoreCase))
        {
            candidates.Add(samName);
        }

        return (candidates, displayName, email);
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
