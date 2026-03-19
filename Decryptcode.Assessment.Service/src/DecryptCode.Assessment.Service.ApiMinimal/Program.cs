using Decryptcode.Assessment.Service.Application;
using Decryptcode.Assessment.Service.Infrastructure.Shared.Constants;
using Decryptcode.Assessment.Service.Infrastructure.SqlServer;
using Decryptcode.Assessment.Service.Infrastructure.SqlServer.Contexts;
using Decryptcode.Assessment.Service.Infrastructure.SqlServer.Seed;
using DecryptCode.Assessment.Service.ApiMinimal.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddLogging();
builder.Services.AddHttpContextAccessor();

builder.Services.AddApplicationServices(builder.Host);

builder.Services.AddSqlServerInfrastructure(builder.Configuration, builder.Environment.IsProduction());

builder.Services.AddHealthChecks();

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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

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
            await context.Database.EnsureCreatedAsync();
        }
        else
        {
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

// Map all endpoints
app.MapOrganizationsEndpoints();
app.MapUsersEndpoints();
app.MapProjectsEndpoints();
app.MapInvoicesEndpoints();
app.MapTimeEntriesEndpoints();
app.MapDashboardEndpoints();
app.MapHealthEndpoints();

app.Run();
