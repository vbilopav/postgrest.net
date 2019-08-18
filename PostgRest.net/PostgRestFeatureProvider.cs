using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Npgsql;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Reflection.Emit;


namespace PostgRest.net
{
    public class PostgRestFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly ILogger<PostgRestFeatureProvider> logger;
        private readonly IServiceCollection services;
        private readonly PostgRestOptions options;
        private readonly ModuleBuilder moduleBuilder;
        private readonly Type typeGet;
        private readonly Type typePost;
        private readonly Type typePut;
        private readonly Type typeDelete;

        public PostgRestFeatureProvider(IServiceCollection services, PostgRestOptions options)
        {
            this.services = services;
            this.options = options;

            var dynamicAssemblyAssemblyName = new AssemblyName("PostgRest.net");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(dynamicAssemblyAssemblyName, AssemblyBuilderAccess.RunAndCollect);
            this.moduleBuilder = assemblyBuilder.DefineDynamicModule(dynamicAssemblyAssemblyName.Name);

            this.typeGet = typeof(GetController<>);
            this.typePost = typeof(PostController<>);
            this.typePut = typeof(PutController<>);
            this.typeDelete = typeof(DeleteController<>);

            using (var builder = this.services.BuildServiceProvider())
            {
                logger = builder.GetService<ILogger<PostgRestFeatureProvider>>();
            }
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            using (var builder = services.BuildServiceProvider())
            using (var connection = builder.GetService<NpgsqlConnection>())
            using (var cmd = new NpgsqlCommand(options.Config.ReadPgRoutinesCommand, connection))
            {
                cmd.Parameters.AddWithValue("schema", options.Schema);
                connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AddControllerFromReader(feature, reader);
                    }
                }
            }
        }

        private void AddControllerFromReader(ControllerFeature feature, NpgsqlDataReader reader)
        {
            if (!(reader["routine_name"] is string name) || !name.StartsWith(options.Prefix))
            {
                return;
            }

            var (routeName, routeType) = GetRouteNameAndType(name);
            if (routeName == null)
            {
                return;
            }

            AddControllerFeature(feature, new ControllerInfo
            {
                RoutineName = name,
                RouteName = routeName,
                RouteType = routeType,
                ReturnType = reader["return_type"] as string,
                Parameters =
                    JsonConvert.DeserializeObject<IEnumerable<Parameter>>(reader["parameters"] as string)
                    .OrderBy(p => p.Position)
                    .Select(p => {
                        var parameterLower = p.ParamName.ToLower();
                        p.ParamNameLower = parameterLower;
                        p.FromQueryString = options.IsQueryStringParameterWhen(p, name);
                        p.FromBody = options.IsBodyParameterWhen(p, name);
                        return p;
                    })
                    .ToList()
            });
        }

        private void AddControllerFeature(ControllerFeature feature, ControllerInfo info)
        {
            var name = $"Proxy{info.RoutineName.GetHashCode()}";
            var typeBuilder = moduleBuilder.DefineType(name, TypeAttributes.Public);
            var type = typeBuilder.CreateTypeInfo();
            feature.Controllers.Add(info.RouteType.MakeGenericType(type).GetTypeInfo());
            info.Options = options;
            info.MatchParamsByQueryStringKeyName = options.MatchParamsByQueryStringKeyNameWhen == null ? false : options.MatchParamsByQueryStringKeyNameWhen(info);
            info.MatchParamsByBodyKeyName = options.MatchParamsByBodyKeyNameWhen == null ? false : options.MatchParamsByBodyKeyNameWhen(info);
            ControllerData.Data.TryAdd(name, info);
        }

        private (string, Type) GetRouteNameAndType(string name)
        {
            var prefix = options.Prefix ?? "";
            string LogString(string route, string verb) =>
                $"Mapping PostgresSQL function \"{name}\" to REST API endpoint \"{verb} {route}\" ...";

            string candidate = name.RemoveFromStart(prefix);
            string candidateLower = candidate.ToLower();
            if (options.IsGetRouteWhen(candidateLower, name))
            {
                var routeName = string.Format(options.RouteNamePattern, options.ResolveRouteName(name, candidate, candidateLower, "GET"));
                logger.LogInformation(LogString(routeName, "GET"));
                return (routeName, typeGet);
            }
            if (options.IsPostRouteWhen(candidateLower, name))
            {
                var routeName = string.Format(options.RouteNamePattern, options.ResolveRouteName(name, candidate, candidateLower, "POST"));
                logger.LogInformation(LogString(routeName, "POST"));
                return (routeName, typePost);
            }
            if (options.IsPutRouteWhen(candidateLower, name))
            {
                var routeName = string.Format(options.RouteNamePattern, options.ResolveRouteName(name, candidate, candidateLower, "PUT"));
                logger.LogInformation(LogString(routeName, "PUT"));
                return (routeName, typePut);
            }
            if (options.IsDeleteRouteWhen(candidateLower, name))
            {
                var routeName = string.Format(options.RouteNamePattern, options.ResolveRouteName(name, candidate, candidateLower, "DELETE"));
                logger.LogInformation(LogString(routeName, "DELETE"));
                return (routeName, typeDelete);
            }
            logger.LogWarning($"Routine {name} skipped, couldn't map to appropriate route. Check Is[Verb]When option.");
            return (null, null);
        }
    }
}
