using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;

namespace PostgRest.net
{
    public class PostgRestOptions
    {
        public string Connection { get;  set; }
        public string Prefix { get; set; } = "rest__";
        public string Schema { get; set; } = "public";
        public IRouteNameResolver RouteNameResolver { get; set; } = new KebabCaseRouteNameResolver();
        public string RouteNamePattern { get; set; } = "api/{0}";
        public Action<IList<IFilterMetadata>, string, string> ApplyFilters { get; set; } = (filter, route, routine) => { };
        public Func<string, string, string> ApplyRouteName { get; set; } = (route, routine) => route;
    }
}