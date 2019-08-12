using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace PostgRest.net
{
    internal class PgFuncParam
    {
        public string ParamName { get; set; }
        public string ParamNameLower { get; set; }
        public string ParamType { get; set; }
        public int Position { get; set; }
        public bool HaveDefault { get; set; }
    }

    internal class ControllerInfo
    {
        public string RoutineName { get; set; }
        public string RouteName { get; set; }
        public Type RouteType { get; set; }
        public string ReturnType { get; set; }
        public IList<PgFuncParam> Parameters { get; set; }
        public PostgRestOptions Options { get; internal set; }
    }

    internal static class ControllerData
    {
        public static ConcurrentDictionary<string, ControllerInfo> Data { get; } = new ConcurrentDictionary<string, ControllerInfo>();
    }
}
