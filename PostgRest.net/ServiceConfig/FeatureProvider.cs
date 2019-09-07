using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;
using PostgRest.Net.Config;
using PostgRest.Net.Controllers;

namespace PostgRest.Net.ServiceConfig
{
    public class FeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly ILogger<FeatureProvider> logger;
        private readonly IServiceCollection services;
        private readonly PostgRestOptions options;
        private readonly ModuleBuilder moduleBuilder;
        private readonly Type typeGet;
        private readonly Type typePost;
        private readonly Type typePut;
        private readonly Type typeDelete;

        public FeatureProvider(IServiceCollection services, PostgRestOptions options)
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
                logger = builder.GetService<ILogger<FeatureProvider>>();
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

            var (routeName, routeType, verb) = GetRouteNameAndType(name);
            if (routeName == null)
            {
                return;
            }

            var parameters =
                JsonConvert.DeserializeObject<IEnumerable<Parameter>>(reader["parameters"] as string)
                    .OrderBy(p => p.Position)
                    .Select(p =>
                    {
                        var parameterLower = p.ParamName.ToLower();
                        p.ParamNameLower = parameterLower;
                        p.FromQueryString = options.IsQueryStringParameterWhen(p, name);
                        p.FromBody = options.IsBodyParameterWhen(p, name);
                        return p;
                    })
                    .ToList();
            AddControllerFeature(feature, new ControllerInfo
            {
                RoutineName = name,
                RouteName = routeName,
                RouteType = routeType,
                ReturnType = reader["return_type"] as string,
                Verb = (Verb)verb,
                Parameters = parameters
            });
        }

        private void AddControllerFeature(ControllerFeature feature, ControllerInfo info)
        {
            var name = $"Proxy{info.RoutineName.GetHashCode()}_{Guid.NewGuid().ToString().Split('-').First()}";
            var typeBuilder = moduleBuilder.DefineType(name, TypeAttributes.Public);
            var type = typeBuilder.CreateTypeInfo();
            feature.Controllers.Add(info.RouteType.MakeGenericType(type).GetTypeInfo());
            info.Options = options;
            info.MatchParamsByQueryStringKey = options.MatchParamsByQueryStringKeyWhen?.Invoke(info) ?? false;
            info.MatchParamsByFormKey = options.MatchParamsByFormKeyWhen?.Invoke(info) ?? false;
            if (!info.MatchParamsByQueryStringKey && !info.MatchParamsByFormKey)
            {

                info.RouteName = string.Concat(info.RouteName,
                    string.Join("", 
                        info.Parameters.Where(p => !p.FromQueryString && !p.FromBody && p.Direction == "IN").Select(p => $"{{{p.ParamName}}}/")));
            }
            var verb = info.Verb.ToString().ToUpper();
            var paramsDesc = info.Parameters.Select(
                p =>
                {
                    var comment = "";
                    if (p.FromQueryString)
                    {
                        comment = string.Concat(comment, "/*from query string*/");
                    }
                    if (p.FromBody)
                    {
                        comment = string.Concat(comment, "/*from body*/");
                    }
                    return $"{comment}{p.Direction} {p.ParamName} {p.ParamType}";
                })
                .ToArray();
            var routine = $"function {info.RoutineName}({string.Join(", ", paramsDesc)}) returns {info.ReturnType}";
            logger.LogInformation($"Mapping PostgreSQL routine \"{routine}\" to REST API endpoint \"{verb} {info.RouteName}\" ... \n");
            ControllerData.Data.TryAdd(name, info);
        }

        private (string, Type, Verb?) GetRouteNameAndType(string name)
        {
            var prefix = options.Prefix ?? "";
            var candidate = name.RemoveFromStart(prefix);
            var candidateLower = candidate.ToLower();
            if (options.IsGetRouteWhen(candidateLower, name))
            {
                var routeName = string.Format(options.RouteNamePattern, options.ResolveRouteName(name, candidate, candidateLower, "GET"));
                return (routeName, typeGet, Verb.Get);
            }
            if (options.IsPostRouteWhen(candidateLower, name))
            {
                var routeName = string.Format(options.RouteNamePattern, options.ResolveRouteName(name, candidate, candidateLower, "POST"));
                return (routeName, typePost, Verb.Post);
            }
            if (options.IsPutRouteWhen(candidateLower, name))
            {
                var routeName = string.Format(options.RouteNamePattern, options.ResolveRouteName(name, candidate, candidateLower, "PUT"));
                return (routeName, typePut, Verb.Put);
            }
            if (options.IsDeleteRouteWhen(candidateLower, name))
            {
                var routeName = string.Format(options.RouteNamePattern, options.ResolveRouteName(name, candidate, candidateLower, "DELETE"));
                return (routeName, typeDelete, Verb.Delete);
            }
            logger.LogWarning($"Routine {name} skipped, couldn't map to appropriate route. Check Is[Verb]When option.");
            return (null, null, null);
        }
    }
}
