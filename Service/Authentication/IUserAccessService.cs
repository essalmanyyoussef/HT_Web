using HT_Web.Models.Authentication;

namespace HT_Web.Services.Authentication;

public interface IUserAccessService
{
    Task<UserContext?> GetCurrentUserAsync();
}