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
using System.Security;
using Microsoft.AspNetCore.Mvc;

namespace PostgRest.net
{
    internal class PgFuncParam
    {
        public string ParamName { get; set; }
        public string ParamType { get; set; }
        public int Position { get; set; }
    }

    internal class ControllerInfo
    {
        public string RoutineName { get; set; }
        public string RouteName { get; set; }
        public string Verb { get; set; }
        public Type RouteType { get; set; }
        public string ReturnType { get; set; }
        public IEnumerable<PgFuncParam> Parameters { get; set; }
    }

    internal static class ControllerData
    {
        public static ConcurrentDictionary<string, ControllerInfo> Data { get; } = new ConcurrentDictionary<string, ControllerInfo>();
    }

    public class PostgRestFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private const string Command =
            @"select
                r.routine_name,
                r.data_type as ""return_type"",
                coalesce(json_agg(
                    json_build_object(
                        'name', p.parameter_name,
                        'type', p.data_type,
                        'position', p.ordinal_position)
                )  filter (where p.parameter_name is not null), '[]') as ""parameters""
            from information_schema.routines r
            left outer join information_schema.parameters p on r.specific_name = p.specific_name
            where
                r.routine_type = 'FUNCTION'
                and r.specific_schema = @schema
            group by
                r.routine_name, r.data_type";

        private readonly ILogger<PostgRestFeatureProvider> logger;
        private readonly IServiceCollection services;
        private readonly PostRestOptions options;
        private readonly ModuleBuilder moduleBuilder;
        private readonly Type typeGet;
        private readonly Type typePost;
        private readonly Type typePut;
        private readonly Type typeDelete;

        public PostgRestFeatureProvider(IServiceCollection services, PostRestOptions options)
        {
            this.services = services;
            this.options = options;

            var dynamicAssemblyAssemblyName = new AssemblyName("PostgRest.net");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(dynamicAssemblyAssemblyName, AssemblyBuilderAccess.RunAndCollect);
            this.moduleBuilder = assemblyBuilder.DefineDynamicModule(dynamicAssemblyAssemblyName.Name);

            this.typeGet = typeof(PgGetController);
            this.typePost = typeof(PgPostController);
            this.typePut = typeof(PgPutController);
            this.typeDelete = typeof(PgDeleteController);

            using (var builder = this.services.BuildServiceProvider())
                logger = builder.GetService<ILogger<PostgRestFeatureProvider>>();
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            RemoveAllExistingControllers(feature);

            using (var builder = services.BuildServiceProvider())
            using (var connection = builder.GetService<NpgsqlConnection>())
            using (var cmd = new NpgsqlCommand(Command, connection))
            {
                cmd.Parameters.AddWithValue("schema", options.Schema);
                connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AddControllersFromReader(feature, reader);
                    }
                }
            }
        }

        private void AddControllersFromReader(ControllerFeature feature, NpgsqlDataReader reader)
        {
            if (!(reader["routine_name"] is string name) || !name.StartsWith(options.Prefix))
            {
                return;
            }

            var (routeName, verb, routeType) = GetRouteNameAndType(name);
            if (routeName == null)
            {
                return;
            }

            feature.Controllers.Add(GetControllerTypeInfo(new ControllerInfo
            {
                RoutineName = name,
                RouteName = routeName,
                Verb = verb,
                RouteType = routeType,
                ReturnType = reader["return_type"] as string,
                Parameters = JsonConvert.DeserializeObject<IEnumerable<PgFuncParam>>(reader["parameters"] as string).ToList()
                    .OrderBy(p => p.Position)
            }));
        }

        private void RemoveAllExistingControllers(ControllerFeature feature)
        {
            feature.Controllers.Remove(typeGet.GetTypeInfo());
            feature.Controllers.Remove(typePost.GetTypeInfo());
            feature.Controllers.Remove(typePut.GetTypeInfo());
            feature.Controllers.Remove(typeDelete.GetTypeInfo());
        }

        private TypeInfo GetControllerTypeInfo(ControllerInfo info)
        {
            var controllerTypeBuilder = moduleBuilder.DefineType($"{info.RouteType.Name}Proxy{info.RoutineName.GetHashCode()}", TypeAttributes.Public, typeGet);
            var routeAttributeBuilder = new CustomAttributeBuilder(typeof(RouteAttribute).GetConstructor(
                new[] {typeof(string)}),
                new object[] { info.RouteName });

            controllerTypeBuilder.SetCustomAttribute(routeAttributeBuilder);
            var result = controllerTypeBuilder.CreateTypeInfo();
            ControllerData.Data.TryAdd(result.Name, info);
            return result;
        }

        private (string, string, Type) GetRouteNameAndType(string name)
        {
            var prefix = options.Prefix ?? "";
            string LogString(string route, string verb) =>
                $"Mapping PostgresSQL function \"{name}\" to REST API endpoint \"{verb} {route}\" ...";

            string candidate = name.RemoveFromStart(prefix);
            string candidateLower = candidate.ToLower();
            if (candidateLower.StartsWith("get"))
            {
                var routeName =
                    string.Format(options.RouteNamePattern,
                        options.RouteNameResolver.GetRouteName(name, candidate, candidateLower.RemoveFromStart("get"), "GET"));
                logger.LogInformation(LogString(routeName, "GET"));
                return (routeName, "GET", typeGet);
            }
            if (candidateLower.StartsWith("post"))
            {
                var routeName =
                    string.Format(options.RouteNamePattern,
                    options.RouteNameResolver.GetRouteName(name, candidate, candidateLower.RemoveFromStart("post"), "POST"));
                logger.LogInformation(LogString(routeName, "POST"));
                return (routeName, "POST", typePost);
            }
            if (candidateLower.StartsWith("put"))
            {
                var routeName =
                    string.Format(options.RouteNamePattern,
                    options.RouteNameResolver.GetRouteName(name, candidate, candidateLower.RemoveFromStart("put"), "PUT"));
                logger.LogInformation(LogString(routeName, "PUT"));
                return (routeName, "PUT", typePut);
            }
            if (candidateLower.StartsWith("delete"))
            {
                var routeName =
                    string.Format(options.RouteNamePattern,
                    options.RouteNameResolver.GetRouteName(name, candidate, candidateLower.RemoveFromStart("delete"), "DELETE"));
                logger.LogInformation(LogString(routeName, "DELETE"));
                return (routeName, "DELETE", typeDelete);
            }
            return (null, null, null);
        }
    }
}
