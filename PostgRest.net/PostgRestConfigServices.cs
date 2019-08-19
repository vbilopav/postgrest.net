using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using System.Reflection;

namespace PostgRest.net
{
    public static class PostgRestConfigServices
    {
        public static IServiceCollection AddPostgRest(this IServiceCollection services, PostgRestOptions options = null)
        {
            options = services.EnsureOptions(options);
            if (options.Connection != null)
            {
                services.AddScoped<NpgsqlConnection, NpgsqlConnection>(provider =>
                {
                    var config = provider.GetRequiredService<IConfiguration>();
                    var connStr = config.GetPgCloudConnectionString(options.Connection) ?? options.Connection;
                    return new NpgsqlConnection(connStr);
                });
            }
            services.TryAddScoped<IStringDataService, StringDataService>();
            services.TryAddScoped<IStringContentService, StringContentService>();
            services.TryAddScoped<ILoggingService, LoggingService>();
            return services;
        }

        public static IMvcBuilder AddPostgRest(this IMvcBuilder builder, IServiceCollection services, PostgRestOptions options = null)
        {
            options = services.EnsureOptions(options);
            services.AddPostgRest(options);
            var assembly = typeof(PostgRestExtensions).GetTypeInfo().Assembly;
            return builder
                .AddApplicationPart(assembly)
                .ConfigureApplicationPartManager(m => m.FeatureProviders.Add(new PostgRestFeatureProvider(services, options)))
                .AddMvcOptions(o => o.Conventions.Add(new PostgRestConvention(options)));
        }

        private static PostgRestOptions EnsureOptions(this IServiceCollection services, PostgRestOptions options)
        {
            if (options != null)
            {
                return options;
            }
            var provider = services.BuildServiceProvider();
            return new PostgRestOptions(provider.GetRequiredService<IConfiguration>());
        }
    }
}
