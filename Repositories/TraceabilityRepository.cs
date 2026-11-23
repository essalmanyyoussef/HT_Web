using System.Data;
using Dapper;
using HT_Web.Data;
using HT_Web.Models.Traceability;

namespace HT_Web.Repositories;

public class TraceabilityRepository : ITraceabilityRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public TraceabilityRepository(
        SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<TraceabilityRecord>>
        GetTraceabilityByFinalLabelAsync(
            string site,
            string finalLabel)
    {
        using var connection =
            _connectionFactory.CreateConnection(site);

        await connection.OpenAsync();

        const string sql =
"""
SELECT DISTINCT
    Ref             AS Reference,
    BC              AS TravelTicketLabel,
    Test            AS Station,
    IN_Barcode_1    AS SecondLabel,
    IN_Barcode_2    AS ThirdTravelTicketLabel,
    IN_Barcode_3    AS FourthTravelTicketLabel,
    OUT_Barcode     AS FinalLabel,
    DateEnd         AS TestDate,
    Rework          AS Reworked,
    Rework_ID       AS ReworkId,
    Hostname
FROM correlations_trial WITH (NOLOCK)
WHERE OUT_Barcode = @FinalLabel
""";

        var result =
            await connection.QueryAsync<TraceabilityRecord>(
                new CommandDefinition(
                    sql,
                    new
                    {
                        FinalLabel = finalLabel
                    },
                    commandTimeout: 300));

        return result.ToList();
    }

    public async Task<List<TraceabilityRecord>>
        GetTraceabilityByTravelTicketAsync(
            string site,
            string travelTicket)
    {
        using var connection =
            _connectionFactory.CreateConnection(site);

        await connection.OpenAsync();

        const string sql =
"""
SELECT DISTINCT
    Ref             AS Reference,
    BC              AS TravelTicketLabel,
    Test            AS Station,
    IN_Barcode_1    AS SecondLabel,
    IN_Barcode_2    AS ThirdTravelTicketLabel,
    IN_Barcode_3    AS FourthTravelTicketLabel,
    OUT_Barcode     AS FinalLabel,
    DateEnd         AS TestDate,
    Rework          AS Reworked,
    Rework_ID       AS ReworkId,
    Hostname
FROM correlations_trial WITH (NOLOCK)
WHERE BC = @TravelTicket
ORDER BY DateEnd ASC
""";

        var result =
            await connection.QueryAsync<TraceabilityRecord>(
                new CommandDefinition(
                    sql,
                    new
                    {
                        TravelTicket = travelTicket
                    },
                    commandTimeout: 300));

        return result.ToList();
    }

    public async Task<List<SolvedErrorRecord>>
        GetErrorsAsync(
            string site,
            string travelTicket)
    {
        using var connection =
            _connectionFactory.CreateConnection(site);

        await connection.OpenAsync();

        System.Diagnostics.Debug.WriteLine(
            $"Searching Errors By Key = [{travelTicket}]");

        const string sql =
"""
SELECT
    Ref             AS Reference,
    Batch,
    Part            AS TravelTicketLabel,
    Worker          AS WorkerId,
    Type            AS ErrorType,
    Counterpart1    AS Tmp01,
    Counterpart2    AS Tmp02,
    Object,
    Wire1,
    Wire2,
    Way,
    DateBegin       AS StartDate,
    DateEnd         AS EndDate,
    TimeU,
    CPV1,
    CPV2,
    CPV3,
    CPV4,
    Hostname
FROM dbo.solvederrors WITH (NOLOCK)
WHERE Part = @TravelTicket
ORDER BY DateBegin DESC
""";

        var result =
            await connection.QueryAsync<SolvedErrorRecord>(
                new CommandDefinition(
                    sql,
                    new
                    {
                        TravelTicket = travelTicket
                    },
                    commandTimeout: 300));

        return result.ToList();
    }

    public async Task<List<ErrorSearchResult>>
        GetEtStationsAsync(
            string site,
            string travelTicket)
    {
        using var connection =
            _connectionFactory.CreateConnection(site);

        await connection.OpenAsync();

        const string sql =
"""
SELECT
    Hostname,
    DateEnd AS TestDate
FROM correlations_trial
WHERE BC = @TravelTicket
AND (
    Test LIKE '%ET%'
    OR Test LIKE '%ELEC%'
    OR Test LIKE '%ELECTRIC%'
)
""";

        var result =
            await connection.QueryAsync<ErrorSearchResult>(
                new CommandDefinition(
                    sql,
                    new
                    {
                        TravelTicket = travelTicket
                    },
                    commandTimeout: 300));

        return result.ToList();
    }

    public async Task<string?> GetStationFamilyAsync(
        string site,
        string hostname,
        DateTime testDate)
    {
        const string sql = @"
            SELECT TOP (1) Bank
            FROM AggregatedCorrelations WITH (NOLOCK)
            WHERE Hostname = @Hostname
              AND RunTimestamp > @TestDate";

        using var connection = _connectionFactory.CreateConnection(site);
        await connection.OpenAsync();

        return await connection.QueryFirstOrDefaultAsync<string>(
            new CommandDefinition(
                sql,
                new { Hostname = hostname, TestDate = testDate },
                commandTimeout: 300));
    }

    public async Task<(DateTime DateBegin, DateTime DateEnd)?> GetTestDatesAsync(
        string site,
        string hostname,
        DateTime testDate)
    {
        const string sql = @"
            SELECT TOP (1) DateBegin, DateEnd
            FROM dbo.tests WITH (NOLOCK)
            WHERE DateBegin < @TestDate
              AND Hostname = @Hostname
            ORDER BY DateEnd DESC";

        using var connection = _connectionFactory.CreateConnection(site);
        await connection.OpenAsync();

        var row = await connection.QueryFirstOrDefaultAsync<(DateTime DateBegin, DateTime DateEnd)?>(
            new CommandDefinition(
                sql,
                new { Hostname = hostname, TestDate = testDate },
                commandTimeout: 300));

        return row;
    }

    public async Task<List<SolvedErrorRecord>> GetErrorsByDateAsync(
        string site,
        string hostname,
        DateTime dateBegin,
        DateTime dateEnd)
    {
        const string sql = @"
            SELECT
                Ref AS Reference,
                Batch,
                Part AS TravelTicketLabel,
                Worker AS WorkerId,
                Type AS ErrorType,
                Counterpart1 AS Tmp01,
                Counterpart2 AS Tmp02,
                Object,
                Wire1,
                Wire2,
                Way,
                DateBegin AS StartDate,
                DateEnd AS EndDate,
                TimeU,
                CPV1,
                CPV2,
                CPV3,
                CPV4,
                Hostname
            FROM dbo.solvederrors WITH (NOLOCK)
            WHERE Hostname = @Hostname
              AND DateBegin BETWEEN @DateBegin AND @DateEnd
            ORDER BY DateBegin DESC";

        using var connection = _connectionFactory.CreateConnection(site);
        await connection.OpenAsync();

        var result = await connection.QueryAsync<SolvedErrorRecord>(
            new CommandDefinition(
                sql,
                new { Hostname = hostname, DateBegin = dateBegin, DateEnd = dateEnd },
                commandTimeout: 300));

        return result.ToList();
    }
    public async Task LogTrackingAsync(
        string serialNumber,
        string site,
        string username,
        DateTime timestamp,
        string additionalInfo,
        string type)
    {
        using var connection = _connectionFactory.CreateTrackingLogConnection();
        if (connection is null)
            return;

        await connection.OpenAsync();

        var parameters = new DynamicParameters();
        parameters.Add("@SerialNumber", serialNumber, DbType.String);
        parameters.Add("@Site", site, DbType.String);
        parameters.Add("@Username", username, DbType.String);
        parameters.Add("@DateTime", timestamp, DbType.DateTime);
        parameters.Add("@AdditionalInfo", additionalInfo, DbType.String);
        parameters.Add("@Type", type, DbType.String);

        await connection.ExecuteAsync(
            new CommandDefinition(
                "sp_InsertTrackingLog",
                parameters,
                commandType: CommandType.StoredProcedure,
                commandTimeout: 30));
    }
}