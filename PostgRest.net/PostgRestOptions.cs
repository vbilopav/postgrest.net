using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;

namespace PostgRest.net
{
    public class PostgRestOptions
    {
        /// <summary>
        /// Connection string or connection string name.
        /// Name will try to find conection string from configuration and also from Azure PostgreSQL connection string configuration convention (POSTGRESQLCONNSTR_{name})
        /// If provided new NpgsqlConnection object will be injected into services as request-response scoped object.
        /// If not provided PostgRest.net will assume that NpgsqlConnection object is already injected in services.
        /// </summary>
        public string Connection { get; set; }
        /// <summary>
        /// Prefix for PostgreSQL routine name which will be considered to expose as REST endpoint.
        /// Routine name is followed by verb to determine endpoint type (eg "rest__{get|post|put|delete}{route name candidate}")
        /// </summary>
        public string Prefix { get; set; } = "rest__";
        /// <summary>
        /// Default database schema
        /// </summary>
        public string Schema { get; set; } = "public";
        /// <summary>
        /// Provides a method to resolve route name from PostgreSQL routine name
        /// string ResolveRouteName(string routineName, string candidateRaw, string candidateLowerNoVerb, string verb)
        /// Default method produces snake cased name from routine name without verb: rest__get_values_json -> values-json
        /// </summary>
        public IRouteNameResolver RouteNameResolver { get; set; } = new KebabCaseRouteNameResolver();
        /// <summary>
        /// Pattern that forms route name after it is resolved: values-json -> api/values-json
        /// </summary>
        public string RouteNamePattern { get; set; } = "api/{0}";
        /// <summary>
        /// Action to add filter to specific routine indetified with route name and routine name
        /// </summary>
        public Action<IList<IFilterMetadata>, string, string> ApplyFilters { get; set; } = (filters, route, routine) => { };
        /// <summary>
        /// Function that receives route and routine and returns final rout name. Default is unchanged
        /// </summary>
        public Func<string, string, string> ApplyRouteName { get; set; } = (route, routine) => route;
        /// <summary>
        /// Apply specific controller convention (filter, properties, attributes, etc)
        /// </summary>
        public Action<ControllerModel, ControllerInfo> ApplyControllerConvention { get; set; } = (model, info) => { };
        /// <summary>
        ///  Func to decide is parameter going to be deserialized from query string
        /// </summary>
        public Func<Parameter, string, bool> IsQueryStringParameterWhen { get; set; } = (parameter, routine) =>
            parameter.ParamNameLower.Contains("query") && (parameter.ParamType == "json" || parameter.ParamType == "jsonb");
        /// <summary>
        ///   Func to decide is parameter going to be deserialized from body
        /// </summary>
        public Func<Parameter, string, bool> IsBodyParameterWhen { get; set; } = (parameter, routine) =>
            parameter.ParamNameLower.Contains("body") && (parameter.ParamType == "json" || parameter.ParamType == "jsonb");
        /// <summary>
        /// Sets response type parameters for succesuful requests for different routine types
        /// </summary>
        public Action<ControllerInfo, IResponse> SetResponseParameters = (info, contentService) =>
        {
            if (info.ReturnType == "json" || info.ReturnType == "jsonb")
            {
                contentService.SetStatusCode(200).SetContentType("application/json; charset=utf-8").SetDefaultValue("{}");
            }
            else if (info.ReturnType == "void")
            {
                contentService.SetStatusCode(204).SetContentType("text/plain; charset=utf-8").SetDefaultValue(null);
            }
            else
            {
                contentService.SetStatusCode(200).SetContentType("text/plain; charset=utf-8").SetDefaultValue(null);
            }
        };
        /// <summary>
        ///   Func to decide is route a GET route. First param is lowered routine name without prefix
        /// </summary>
        public Func<string, string, bool> IsGetRouteWhen { get; set; } = (candidateLower, routine) =>
            candidateLower.StartsWith("get");
        /// <summary>
        ///   Func to decide is route a POST route. First param is lowered routine name without prefix
        /// </summary>
        public Func<string, string, bool> IsPostRouteWhen { get; set; } = (candidateLower, routine) =>
            candidateLower.StartsWith("post");
        /// <summary>
        ///   Func to decide is route a PUT route. First param is lowered routine name without prefix
        /// </summary>
        public Func<string, string, bool> IsPutRouteWhen { get; set; } = (candidateLower, routine) =>
            candidateLower.StartsWith("put");
        /// <summary>
        ///   Func to decide is route a DELETE route. First param is lowered routine name without prefix
        /// </summary>
        public Func<string, string, bool> IsDeleteRouteWhen { get; set; } = (candidateLower, routine) =>
            candidateLower.StartsWith("delete");
    }
}