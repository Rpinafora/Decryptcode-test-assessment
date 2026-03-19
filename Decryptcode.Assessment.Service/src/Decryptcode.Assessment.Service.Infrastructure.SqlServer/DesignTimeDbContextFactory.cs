using Decryptcode.Assessment.Service.Infrastructure.Shared.Constants;
using Decryptcode.Assessment.Service.Infrastructure.SqlServer.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Decryptcode.Assessment.Service.Infrastructure.SqlServer;

// Design-time factory to allow EF tools to create the DbContext when running migrations.
// It will try to read the connection string from environment variables, appsettings.json files
// (searching up parent directories), or fallback to a LocalDB string.
public partial class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApiContext>
{
    public ApiContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();

        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__" + ApiConstants.SQL_CONNECTION_STRING_NAME)
                               ?? Environment.GetEnvironmentVariable(ApiConstants.SQL_CONNECTION_STRING_NAME);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Server=(localdb)\\mssqllocaldb;Database=Decryptcode_Assessment_LocalDb;Trusted_Connection=True;";
        }

        var optionsBuilder = new DbContextOptionsBuilder<ApiContext>();
        optionsBuilder.UseSqlServer(connectionString);


        var httpAccessor = new DesignTimeHttpContextAccessor();

        return new ApiContext(optionsBuilder.Options, httpAccessor);
    }
}
