using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using PostgRest.Net.Config;

namespace PostgRest.Net.Controllers
{
    public class Parameter
    {
        public string ParamName { get; set; }
        public string ParamNameLower { get; set; }
        public string ParamType { get; set; }
        public int Position { get; set; }
        public bool HaveDefault { get; set; }
        public string Direction { get; set; }
        public bool FromQueryString { get; set; }
        public bool FromBody { get; set; }
    }

    public enum Verb { Get, Post, Put, Delete }

    public class ControllerBaseInfo
    {
        public string RoutineName { get; set; }
        public string RouteName { get; set; }
        public string ReturnType { get; set; }
        public IList<Parameter> Parameters { get; set; }
        public Verb Verb { get; set; }
    }

    public class ControllerInfo : ControllerBaseInfo
    {
        public bool MatchParamsByQueryStringKey { get; set; }
        public bool MatchParamsByFormKey { get; set; }
        public Type RouteType { get; set; }
        public PostgRestOptions Options { get; internal set; }
    }

    internal static class ControllerData
    {
        public static ConcurrentDictionary<string, ControllerInfo> Data { get; } = new ConcurrentDictionary<string, ControllerInfo>();
    }
}
