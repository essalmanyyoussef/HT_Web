namespace HT_Web.Models.Authentication;

public class AuthenticationResponse
{
    public bool HasAccess { get; set; }

    public List<string> AccessibleSites { get; set; } = new();
}