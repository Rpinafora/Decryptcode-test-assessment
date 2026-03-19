using Decryptcode.Assessment.Service.Domain.Repositories;
using Decryptcode.Assessment.Service.Infrastructure.Shared.Constants;
using Decryptcode.Assessment.Service.Infrastructure.SqlServer.Contexts;
using Decryptcode.Assessment.Service.Infrastructure.SqlServer.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Decryptcode.Assessment.Service.Infrastructure.SqlServer;

public static class ServicesInjector
{
    /// <summary>
    /// Adds SQL Server or SQLite database context based on configuration and availability.
    /// If SQL Server connection fails or not configured, falls back to SQLite for local development.
    /// </summary>
    public static void AddSqlServerInfrastructure(this IServiceCollection services, IConfiguration configuration, bool isProduction = false)
    {
        var connectionString = configuration.GetConnectionString(ApiConstants.SQL_CONNECTION_STRING_NAME);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("There is no connection string provided on App Settings");
        }

        if (!isProduction && !IsSqlServerAvailable(connectionString))
        {
            AddSqliteDatabase(services);
        }
        else if (IsSqlServerAvailable(connectionString))
        {
            AddSqlServerDatabase(services, connectionString);
        }

        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITimeEntriesRepository, TimeEntryRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
    }

    private static void AddSqlServerDatabase(IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApiContext>(options =>
            options.UseSqlServer(connectionString,
                sqlOptions => sqlOptions.EnableRetryOnFailure(2, TimeSpan.FromSeconds(30), null)));
    }

    private static void AddSqliteDatabase(IServiceCollection services)
    {
        var sqlitePath = Path.Combine(AppContext.BaseDirectory, "Data");
        Directory.CreateDirectory(sqlitePath);
        var dbPath = Path.Combine(sqlitePath, "assessmentdb.db");
        var connectionString = $"Data Source={dbPath}";

        services.AddDbContext<ApiContext>(options =>
            options.UseSqlite(connectionString,
                sqliteOptions => sqliteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
    }

    private static bool IsSqlServerAvailable(string connectionString)
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
