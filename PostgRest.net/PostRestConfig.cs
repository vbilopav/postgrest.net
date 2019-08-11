using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;
using System.Reflection;

namespace PostgRest.net
{
    public static class PostRestExtensions
    {
        public static IServiceCollection AddPostgRest(this IServiceCollection services, PostRestOptions options = default)
        {
            options = options ?? new PostRestOptions();
            if (options.ConnectionString != null)
            {
                services.AddScoped<NpgsqlConnection, NpgsqlConnection>(provider =>
                {
                    var config = provider.GetRequiredService<IConfiguration>();
                    var connStr = config.GetPgCloudConnectionString(options.ConnectionString) ?? options.ConnectionString;
                    return new NpgsqlConnection(connStr);
                });
            }
            services.AddScoped<IPgDataService, PgDataService>();
            services.AddScoped<IPgDataContentService, PgDataContentService>();
            return services;
        }

        public static IMvcBuilder AddPostgRest(this IMvcBuilder builder, IServiceCollection services, PostRestOptions options = default)
        {
            options = options ?? new PostRestOptions();
            services.AddPostgRest(options);
            var assembly = typeof(PostRestExtensions).GetTypeInfo().Assembly;
            return builder
                .AddApplicationPart(assembly)
                .ConfigureApplicationPartManager(m => m.FeatureProviders.Add(new PostgRestFeatureProvider(services, options)));
        }

        public static string GetPgCloudConnectionString(this IConfiguration config, string connectionStringName) =>
            config.GetConnectionString(connectionStringName) ?? config.GetSection($"POSTGRESQLCONNSTR_{connectionStringName}")?.Value;

        internal static string RemoveFromStart(this string source, string remove) =>
            source.Remove(source.IndexOf(remove, StringComparison.Ordinal), remove.Length);
    }
}
