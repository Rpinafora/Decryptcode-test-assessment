using Decryptcode.Assessment.Service.Api.HealthChecks;
using Decryptcode.Assessment.Service.Api.Swagger;
using Decryptcode.Assessment.Service.Application;
using Decryptcode.Assessment.Service.Infrastructure.Shared.Constants;
using Decryptcode.Assessment.Service.Infrastructure.SqlServer;
using Decryptcode.Assessment.Service.Infrastructure.SqlServer.Contexts;
using Decryptcode.Assessment.Service.Infrastructure.SqlServer.Seed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHealthChecks()
    .AddCheck<DbHealthCheck>("Database");

builder.Services.AddHttpContextAccessor();

builder.Services.AddLogging();

builder.Services.AddSwaggerConfiguration();

builder.Services.AddSqlServerInfrastructure(builder.Configuration, builder.Environment.IsProduction());

builder.Services.AddApplicationServices(builder.Host);

builder.Services.AddRouting();

var enableSwagger = bool.Parse(builder.Configuration.GetValue<string>("EnableSwagger") ?? "false");

builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration
        .GetValue<string>(ApiConstants.ALLOWED_ORIGINS_SECTION_NAME)?
        .Split(';');

    var allowedMethods = builder.Configuration
        .GetValue<string>(ApiConstants.ALLOWED_METHODS_SECTION_NAME)?
        .Split(';');

    if (allowedOrigins is { Length: > 0 })
    {
        options.AddPolicy(ApiConstants.API_CORS_POLICY_NAME, policy =>
        {
            policy.WithOrigins(allowedOrigins)
            .WithMethods(allowedMethods ?? [])
            .AllowAnyHeader();
        });
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAppSwagger(enableSwagger);

app.UseCors(ApiConstants.API_CORS_POLICY_NAME);

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApiContext>();
    var connectionString = context.Database.GetConnectionString() ?? "";

    try
    {
        var isSqlite = connectionString.Contains("Data Source=") && !connectionString.Contains("Server=");

        if (isSqlite)
        {
            // For SQLite, we use EnsureCreatedAsync to bypass migration issues
            // This creates the schema directly from the DbContext model
            await context.Database.EnsureCreatedAsync();
        }
        else
        {
            // For SQL Server, use migrations as normal
            await context.Database.MigrateAsync();
        }

        await DbSeeder.SeedAsync(context);
    }
    catch (Exception ex)
    {
        throw;
    }
}

app.UseRouting();

app.UseHealthChecks("/health");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
