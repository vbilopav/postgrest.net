using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using System;
using System.Reflection;

namespace PostgRest.net
{
    public static class PostgRestConfigServices
    {
        public static IServiceCollection AddPostgRest(this IServiceCollection services, PostgRestOptions options = null)
        {
            options = options ?? new PostgRestOptions();
            if (options.Connection != null)
            {
                services.AddScoped<NpgsqlConnection, NpgsqlConnection>(provider =>
                {
                    var config = provider.GetRequiredService<IConfiguration>();
                    var connStr = config.GetPgCloudConnectionString(options.Connection) ?? options.Connection;
                    return new NpgsqlConnection(connStr);
                });
            }
            services.TryAddScoped<IDataService, DataService>();
            services.TryAddScoped<IContentService, ContentService>();
            services.TryAddScoped<ILoggingService, LoggingService>();
            return services;
        }

        public static IMvcBuilder AddPostgRest(this IMvcBuilder builder, IServiceCollection services, PostgRestOptions options = null)
        {
            options = options ?? new PostgRestOptions();
            services.AddPostgRest(options);
            var assembly = typeof(PostgRestExtensions).GetTypeInfo().Assembly;
            return builder
                .AddApplicationPart(assembly)
                .ConfigureApplicationPartManager(m => m.FeatureProviders.Add(new PostgRestFeatureProvider(services, options)))
                .AddMvcOptions(o => o.Conventions.Add(new PostgRestConvention(options)));
        }
    }
}
