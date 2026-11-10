using System.Xml.Linq;
using HT_Web.Models.Authentication;

namespace HT_Web.Services.Authentication;

public class AuthenticationApiService
{
    private readonly HttpClient _httpClient;

    public AuthenticationApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthenticationResponse> GetSitesAsync(string userName)
    {
        AuthenticationResponse result = new();

        try
        {
            var form = new MultipartFormDataContent();

            form.Add(
                new StringContent(userName),
                "username");

            var response =
                await _httpClient.PostAsync(
                    "https://ukhqengs001.sews-e.com/ElectricalTest/HarnessTraceabilityAuthentication",
                    form);

            if (!response.IsSuccessStatusCode)
                return result;

            string xml =
                await response.Content.ReadAsStringAsync();

            XDocument doc =
                XDocument.Parse(xml);

            var sites =
                doc.Root?.Element("Sites");

            if (sites == null)
                return result;

            foreach (var site in sites.Elements())
            {
                if (site.Value == "True")
                {
                    result.AccessibleSites.Add(site.Name.LocalName);
                }
            }

            result.HasAccess =
                result.AccessibleSites.Count > 0;

            return result;
        }
        catch
        {
            return result;
        }
    }
}