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
        private const string Command =
            @"select
                r.routine_name,
                r.data_type as ""return_type"",
                coalesce(json_agg(
                    json_build_object(
                        'ParamName', p.parameter_name,
                        'ParamType', p.data_type,
                        'Position', p.ordinal_position,
                        'HaveDefault', p.parameter_default is not null)
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

            this.typeGet = typeof(PgGetController<>);
            this.typePost = typeof(PgPostController<>);
            this.typePut = typeof(PgPutController<>);
            this.typeDelete = typeof(PgDeleteController<>);

            using (var builder = this.services.BuildServiceProvider())
            {
                logger = builder.GetService<ILogger<PostgRestFeatureProvider>>();
            }
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
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
                    JsonConvert.DeserializeObject<IEnumerable<PgFuncParam>>(reader["parameters"] as string)
                    .ToList()
                    .OrderBy(p => p.Position)
            });
        }

        private void AddControllerFeature(ControllerFeature feature, ControllerInfo info)
        {
            var name = $"PgCtrlProxy{info.RoutineName.GetHashCode()}";
            var typeBuilder = moduleBuilder.DefineType(name, TypeAttributes.Public);
            var type = typeBuilder.CreateTypeInfo();
            feature.Controllers.Add(info.RouteType.MakeGenericType(type).GetTypeInfo());
            ControllerData.Data.TryAdd(name, info);
        }

        private (string, Type) GetRouteNameAndType(string name)
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
                return (routeName, typeGet);
            }
            if (candidateLower.StartsWith("post"))
            {
                var routeName =
                    string.Format(options.RouteNamePattern,
                    options.RouteNameResolver.GetRouteName(name, candidate, candidateLower.RemoveFromStart("post"), "POST"));
                logger.LogInformation(LogString(routeName, "POST"));
                return (routeName, typePost);
            }
            if (candidateLower.StartsWith("put"))
            {
                var routeName =
                    string.Format(options.RouteNamePattern,
                    options.RouteNameResolver.GetRouteName(name, candidate, candidateLower.RemoveFromStart("put"), "PUT"));
                logger.LogInformation(LogString(routeName, "PUT"));
                return (routeName, typePut);
            }
            if (candidateLower.StartsWith("delete"))
            {
                var routeName =
                    string.Format(options.RouteNamePattern,
                    options.RouteNameResolver.GetRouteName(name, candidate, candidateLower.RemoveFromStart("delete"), "DELETE"));
                logger.LogInformation(LogString(routeName, "DELETE"));
                return (routeName, typeDelete);
            }
            return (null, null);
        }
    }
}
