using Decryptcode.Assessment.Service.Application.Organizations.Queries.GetAllOrganizations;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.FluentValidation;

namespace Decryptcode.Assessment.Service.Application;

public static class ServiceInjector
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IHostBuilder? hostBuilder)
    {
        if (hostBuilder == null)
        {
            return services;
        }

        hostBuilder.UseWolverine(options =>
        {
            options.Durability.Mode = DurabilityMode.Solo;
            options.UseFluentValidation(RegistrationBehavior.ExplicitRegistration);
            options.Discovery.IncludeAssembly(typeof(GetAllOrganizationsQuery).Assembly);

            options.Policies.DisableConventionalLocalRouting();

        });

        services.AddValidatorsFromAssemblyContaining<GetAllOrganizationsQuery>();

        return services;
    }
}
