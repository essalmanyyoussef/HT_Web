using HT_Web.Models.Configuration;
using Microsoft.Data.SqlClient;

namespace HT_Web.Data;

public class SqlConnectionFactory
{
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
}