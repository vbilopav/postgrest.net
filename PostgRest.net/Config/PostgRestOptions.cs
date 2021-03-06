﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using PostgRest.Net.Controllers;
using PostgRest.Net.DataServices;
using PostgRest.Net.ServiceConfig;

namespace PostgRest.Net.Config
{
    public class PostgRestOptions
    {
        public PostgRestConfig Config { get; private set; }
        /// <summary>
        /// Connection string or connection string name.
        /// Name will try to find connection string from configuration and also from Azure PostgreSQL connection string configuration convention (POSTGRESQLCONNSTR_{name})
        /// If provided new NpgsqlConnection object will be injected into services as request-response scoped object.
        /// If not provided PostgRest.net will assume that NpgsqlConnection object is already injected in services.
        /// </summary>
        public string Connection { get; set; }
        /// <summary>
        /// Prefix for PostgreSQL routine name which will be considered to expose as REST endpoint.
        /// Routine name is followed by verb to determine endpoint type (eg "rest__{get|post|put|delete}{route name candidate}")
        /// </summary>
        public string Prefix { get; set; }
        /// <summary>
        /// Default database schema
        /// </summary>
        public string Schema { get; set; }
        /// <summary>
        /// Provides a method to resolve route name from PostgreSQL routine name
        /// Default method produces snake cased name from routine name without verb: rest__get_values_json -> values-json
        /// </summary>
        public Func<string, string, string, string, string> ResolveRouteName { get; set; }
        /// <summary>
        /// Pattern that forms route name after it is resolved: values-json -> api/values-json
        /// </summary>
        public string RouteNamePattern { get; set; }
        /// <summary>
        /// Action to add filter to specific routine identified with route name and routine name
        /// </summary>
        public Action<IList<IFilterMetadata>, string, string> ApplyFilters { get; set; }
        /// <summary>
        /// Function that receives route and routine and returns final rout name. Default is unchanged
        /// </summary>
        public Func<string, string, string> ApplyRouteName { get; set; }
        /// <summary>
        /// Apply specific controller convention (filter, properties, attributes, etc)
        /// </summary>
        public Action<ControllerModel, ControllerBaseInfo> ApplyControllerConvention { get; set; }
        /// <summary>
        ///  Func to decide is parameter going to be deserialized from query string
        /// </summary>
        public Func<Parameter, string, bool> IsQueryStringParameterWhen { get; set; }
        /// <summary>
        ///   Func to decide is parameter going to be deserialized from body
        /// </summary>
        public Func<Parameter, string, bool> IsBodyParameterWhen { get; set; }
        /// <summary>
        /// Sets response parameters (status code, content type and value for null result) - for successful requests for different routine types
        /// </summary>
        public Action<ControllerBaseInfo, IResponse> SetResponseParameters { get; set; }
        /// <summary>
        ///   Func to decide is route a GET route. First param is lowered routine name without prefix
        /// </summary>
        public Func<string, string, bool> IsGetRouteWhen { get; set; }
        /// <summary>
        ///   Func to decide is route a POST route. First param is lowered routine name without prefix
        /// </summary>
        public Func<string, string, bool> IsPostRouteWhen { get; set; }
        /// <summary>
        ///   Func to decide is route a PUT route. First param is lowered routine name without prefix
        /// </summary>
        public Func<string, string, bool> IsPutRouteWhen { get; set; }
        /// <summary>
        ///   Func to decide is route a DELETE route. First param is lowered routine name without prefix
        /// </summary>
        public Func<string, string, bool> IsDeleteRouteWhen { get; set; }
        /// <summary>
        ///  update parameter with custom value by setting first parameter ReferenceValueType -> value.Value = "some value";
        /// </summary>
        public Action<ReferenceValueType, string, ControllerBaseInfo, Microsoft.AspNetCore.Mvc.ControllerBase> ApplyParameterValue { get; set; }
        /// <summary>
        /// Decide should routine parameters be mapped by each name from query string
        /// </summary>
        public Func<ControllerBaseInfo, bool> MatchParamsByQueryStringKeyWhen { get; set; }
        /// <summary>
        /// Decide should routine parameters be mapped by each name from body
        /// </summary>
        public Func<ControllerBaseInfo, bool> MatchParamsByFormKeyWhen { get; set; }

        public PostgRestOptions(IConfiguration configuration = null)
        {
            Config = new PostgRestConfig();
            configuration?.Bind("PostgRest", Config);

            Connection = Config.Connection;
            Prefix = Config.RoutinePrefix;
            Schema = Config.DatabaseSchema;

            ResolveRouteName = (routine, routineNoPrefix, routineLowerNoPrefix, verb) =>
            {
                // resolve to kebab by default
                var snaked = string.Concat(routineLowerNoPrefix.RemoveFromStart(verb.ToLower()).Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString()));
                var result = snaked.Trim('_').Replace("_", "-");
                return result.EndsWith("/") ? result : string.Concat(result, "/");
            };

            RouteNamePattern = Config.RouteNamePattern;
            ApplyFilters = (filters, route, routine) => { };
            ApplyRouteName = (route, routine) => route;
            ApplyControllerConvention = (model, info) => { };

            IsQueryStringParameterWhen = (parameter, routine) =>
                Regex.Match(parameter.ParamNameLower, Config.QueryStringParamNameRegex, RegexOptions.IgnoreCase).Success && 
                Config.JsonTypes.Contains(parameter.ParamType);

            IsBodyParameterWhen = (parameter, routine) =>
                Regex.Match(parameter.ParamNameLower, Config.BodyParamNameRegex, RegexOptions.IgnoreCase).Success &&
                (Config.JsonTypes.Contains(parameter.ParamType) || parameter.ParamType == "text");

            SetResponseParameters = (info, contentService) =>
            {
                if (Config.JsonTypes.Contains(info.ReturnType))
                {
                    contentService.SetStatusCode(200).SetContentType(Config.JsonContentType).SetDefaultValue(Config.JsonDefaultValue);
                }
                else if (info.ReturnType == Config.VoidType)
                {
                    contentService.SetStatusCode(204).SetContentType(Config.TextContentType).SetDefaultValue(Config.NonJsonDefaultValue);
                }
                else
                {
                    contentService.SetStatusCode(200).SetContentType(Config.TextContentType).SetDefaultValue(Config.NonJsonDefaultValue);
                }
            };

            IsGetRouteWhen = (candidateLower, routine) => Regex.Match(candidateLower, Config.GetRouteRegex, RegexOptions.IgnoreCase).Success;
            IsPostRouteWhen = (candidateLower, routine) => Regex.Match(candidateLower, Config.PostRouteRegex, RegexOptions.IgnoreCase).Success;
            IsPutRouteWhen = (candidateLower, routine) => Regex.Match(candidateLower, Config.PutRouteRegex, RegexOptions.IgnoreCase).Success;
            IsDeleteRouteWhen = (candidateLower, routine) => Regex.Match(candidateLower, Config.DeleteRouteRegex, RegexOptions.IgnoreCase).Success;

            ApplyParameterValue = null;
            MatchParamsByQueryStringKeyWhen = null;
            MatchParamsByFormKeyWhen = null;
        }
    }
}