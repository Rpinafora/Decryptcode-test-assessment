using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Decryptcode.Assessment.Service.Api.Swagger;

public static class SwaggerConfiguration
{
    private const string SwaggerDocName = "v1";
    private const string Bearer = "Bearer";

    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        return services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(SwaggerDocName, new OpenApiInfo { Title = "Decryptcode Assessment Service API", Version = SwaggerDocName });

            options.CustomSchemaIds(type => type.FullName!.Replace('+', '.'));

            options.EnableAnnotations();

            options.TagActionsBy(ResolveTags);

            var securityScheme = new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = Bearer
            };

            options.AddSecurityDefinition(Bearer, securityScheme);
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                { new OpenApiSecuritySchemeReference(Bearer, null), new List<string>() }
            });
        });
    }

    public static IApplicationBuilder UseAppSwagger(this IApplicationBuilder app, bool enableSwagger)
    {
        app.UseSwagger();

        if (!enableSwagger)
        {
            return app;
        }

        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/")
            {
                context.Response.Redirect("/swagger");

            }
            await next();
        });

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint($"/swagger/{SwaggerDocName}/swagger.json", "Decryptcode Assessment Service API");
        });

        return app;
    }

    private static string[] ResolveTags(ApiDescription apiDescription)
    {
        var byAttributes = TryGetTagsFromAttributes(apiDescription);
        if (byAttributes.Length > 0)
        {
            return byAttributes;
        }

        var byRoute = TryGetTagsFromRoute(apiDescription);
        if (byRoute.Length > 0)
        {
            return byRoute;
        }

        var fallback = apiDescription.GroupName ?? apiDescription.ActionDescriptor.RouteValues["controller"] ?? "General";

        return [fallback];
    }

    private static string[] TryGetTagsFromAttributes(ApiDescription apiDescription)
    {
        if (!apiDescription.TryGetMethodInfo(out var info))
        {
            return [];
        }

        var tags = info
            .GetCustomAttributes<SwaggerOperationAttribute>()
            .SelectMany(a => a.Tags ?? [])
            .Select(t =>
            {
                var trimmed = t.Trim();
                if (string.IsNullOrWhiteSpace(trimmed))
                {
                    return null;
                }

                var index = trimmed.IndexOf(':');

                return index > 0 ? trimmed[..index].Trim() : trimmed;
            })
            .Where(a => !string.IsNullOrWhiteSpace(a))
            .Select(a => a!)
            .Distinct()
            .ToArray();

        return tags.Length > 0 ? tags : [];
    }

    private static string[] TryGetTagsFromRoute(ApiDescription apiDescription)
    {
        var path = apiDescription.RelativePath?.ToLowerInvariant();

        if (string.IsNullOrEmpty(path) || !path.StartsWith("api/"))
        {
            return [];
        }

        var first = path.Split('/', StringSplitOptions.RemoveEmptyEntries)
            .ElementAtOrDefault(1);

        var group = first
        switch
        {
            "organizations" => "Organizations",
            "users" => "Users",
            "projects" => "Projects",
            "invoices" => "Invoices",
            "health" => "Health",
            _ => throw new NotImplementedException()
        };

        return string.IsNullOrWhiteSpace(group) ? [] : [group];
    }
}
