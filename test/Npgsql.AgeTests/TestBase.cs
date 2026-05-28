using Microsoft.Extensions.Configuration;
using Npgsql.Age;

namespace Npgsql.AgeTests;

public class TestBase
{
    private readonly NpgsqlDataSource _dataSource;
    private static readonly Lazy<Task<Version>> _ageVersion = new(GetAgeVersionAsync);

    protected static async Task<bool> AgeVersionSupportsTypeCasts()
    {
        var version = await _ageVersion.Value;
        return version >= new Version(1, 6);
    }

    private static async Task<Version> GetAgeVersionAsync()
    {
        await using var dataSource = new NpgsqlDataSourceBuilder(
            new NpgsqlConnectionStringBuilder(
                new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.Development.json")
                    .Build()
                    .GetConnectionString("AgeConnectionString")
                    ?? throw new ArgumentNullException("AgeConnectionString")
            ) { SearchPath = "ag_catalog, \"$user\", public" }.ConnectionString
        ).Build();
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT extversion FROM pg_extension WHERE extname = 'age'";
        var versionStr = (await cmd.ExecuteScalarAsync())?.ToString() ?? "1.5.0";
        var parts = versionStr.Split('.');
        return parts.Length >= 2
            ? new Version(int.Parse(parts[0]), int.Parse(parts[1]))
            : new Version(1, 5);
    }

    public TestBase()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Development.json")
            .Build();

        string connectionString =
            configuration.GetConnectionString("AgeConnectionString")
            ?? throw new ArgumentNullException("AgeConnectionString");

        NpgsqlConnectionStringBuilder connectionStringBuilder =
            new(connectionString) { SearchPath = "ag_catalog, \"$user\", public" };
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(
            connectionStringBuilder.ConnectionString
        );
        // UseAge(true) for CNPG images, controlled by CNPG_TEST env var
        var cnpgTest = Environment.GetEnvironmentVariable("CNPG_TEST");
        if (!string.IsNullOrEmpty(cnpgTest) && cnpgTest.ToLowerInvariant() == "true")
        {
            _dataSource = dataSourceBuilder.UseAge(true).Build();
        }
        else
        {
            _dataSource = dataSourceBuilder.UseAge().Build();
        }
    }

    public void Dispose()
    {
        _dataSource?.Dispose();
    }

    public NpgsqlDataSource DataSource => _dataSource;

    protected async Task<string> CreateTempGraphAsync()
    {
        var graphName = "temp_graph" + DateTime.Now.ToString("yyyyMMddHHmmssffff");
        await using var connection = await DataSource.OpenConnectionAsync();
        await using var command = connection.CreateGraphCommand(graphName);
        await command.ExecuteNonQueryAsync();
        return graphName;
    }

    protected async Task DropTempGraphAsync(string graphName)
    {
        await using var connection = await DataSource.OpenConnectionAsync();
        await using var command = connection.DropGraphCommand(graphName);
        await command.ExecuteNonQueryAsync();
    }
}
