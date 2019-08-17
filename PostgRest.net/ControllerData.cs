using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace PostgRest.net
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

    public class ControllerInfo
    {
        public string RoutineName { get; set; }
        public string RouteName { get; set; }
        public Type RouteType { get; set; }
        public string ReturnType { get; set; }
        public IList<Parameter> Parameters { get; set; }
        public PostgRestOptions Options { get; internal set; }
    }

    internal static class ControllerData
    {
        private static PostgRestConfig config;

        public static PostgRestConfig Config
        {
            get => config ?? new PostgRestConfig();
            set => config = value;
        }
        public static ConcurrentDictionary<string, ControllerInfo> Data { get; } = new ConcurrentDictionary<string, ControllerInfo>();
    }
}
