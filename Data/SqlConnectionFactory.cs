using HT_Web.Models.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace HT_Web.Data;

public class SqlConnectionFactory
{
    private readonly IConfiguration _configuration;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public SqlConnection CreateConnection(string site)
    {
        var config =
            SiteConfigurations.Sites
            .First(x => x.Site == site);

        var connectionString =
            $"Server={config.Server};" +
            $"Database={config.Database};" +
            $"User Id={config.Username};" +
            $"Password={config.Password};" +
            $"TrustServerCertificate=True;";

        return new SqlConnection(connectionString);
    }

    public SqlConnection? CreateTrackingLogConnection()
    {
        var connStr = _configuration.GetConnectionString("TrackingLog");
        if (string.IsNullOrWhiteSpace(connStr))
            return null;

        return new SqlConnection(connStr);
    }
}