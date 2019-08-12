using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;
using System.Reflection;

namespace PostgRest.net
{
    public static class PostgRestExtensions
    {
        public static IServiceCollection AddPostgRest(this IServiceCollection services, PostgRestOptions options = default)
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
            services.AddScoped<IPgDataService, PgDataService>();
            services.AddScoped<IPgDataContentService, PgDataContentService>();
            return services;
        }

        public static IMvcBuilder AddPostgRest(this IMvcBuilder builder, IServiceCollection services, PostgRestOptions options = default)
        {
            options = options ?? new PostgRestOptions();
            services.AddPostgRest(options);
            var assembly = typeof(PostgRestExtensions).GetTypeInfo().Assembly;
            return builder
                .AddApplicationPart(assembly)
                .ConfigureApplicationPartManager(m => m.FeatureProviders.Add(new PostgRestFeatureProvider(services, options)))
                .AddMvcOptions(o => o.Conventions.Add(new PostgRestRouteConvention()));
        }

        public static string GetPgCloudConnectionString(this IConfiguration config, string connectionStringName) =>
            config.GetConnectionString(connectionStringName) ?? config.GetSection($"POSTGRESQLCONNSTR_{connectionStringName}")?.Value;

        internal static string RemoveFromStart(this string source, string remove) =>
            source.Remove(source.IndexOf(remove, StringComparison.Ordinal), remove.Length);
    }
}
