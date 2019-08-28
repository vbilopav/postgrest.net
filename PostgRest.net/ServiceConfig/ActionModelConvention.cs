using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace PostgRest.net
{
    public static class ParameterInfoFactory
    {
        private delegate void EventType(int p0, long p1, short p2, bool p3, DateTime p4, decimal p5, float p6, string p7);
        #pragma warning disable 67
        private static event ParameterInfoFactory.EventType Event;
        #pragma warning restore 67
        private static readonly ParameterInfo[] Info;

        static ParameterInfoFactory()
        {
            var delegateType = typeof(ParameterInfoFactory).GetEvent("Event", BindingFlags.NonPublic | BindingFlags.Static)?.EventHandlerType;
            if (delegateType != null)
            {
                var invoke = delegateType.GetMethod("Invoke");
                Info = invoke?.GetParameters();
            }
        }

        public static ParameterInfo GetParameterInfoByPgName(string name)
        {
            if (name == "integer" || name == "int" || name == "int4")
            {
                return Info[0];
            }
            if (name == "bigint" || name == "int8" || name == "bigserial" || name == "serial8")
            {
                return Info[1];
            }
            if (name == "smallint" || name == "int2")
            {
                return Info[2];
            }
            if (name == "boolean" || name == "bool")
            {
                return Info[3];
            }
            if (name == "timestamp" || 
                name == "timestamp without time zone" || 
                name == "timestamp with time zone" || 
                name == "timestampz" || 
                name == "time" ||
                name == "time without time zone" ||
                name == "time with time zone" ||
                name == "timez" ||
                name == "date")
            {
                return Info[4];
            }
            if (name == "numeric" || name == "decimal")
            {
                return Info[5];
            }
            if (name == "double precision" || name == "float8")
            {
                return Info[6];
            }
            return Info[7];
        }
    }

    public class ActionModelConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            if (action?.Controller == null)
            {
                return;
            }
            var controller = action.Controller;
            if (!controller.ControllerType.IsGenericType)
            {
                return;
            }
            var genericType = controller.ControllerType.GenericTypeArguments[0];
            if (!ControllerData.Data.TryGetValue(genericType.Name, out var info))
            {
                return;
            }

            foreach (var parameter in info.Parameters)
            {
                var source = BindingSource.Path;
                if (parameter.FromBody)
                {
                    source = BindingSource.Body;
                }
                else if (parameter.FromQueryString)
                {
                    source = BindingSource.Query;
                }
                var pm = new ParameterModel(ParameterInfoFactory.GetParameterInfoByPgName(parameter.ParamType),
                    new List<object>())
                {
                    ParameterName = parameter.ParamName,
                    BindingInfo = new BindingInfo
                    {
                        BindingSource = source,
                        RequestPredicate = c => false
                    }
                };
                action.Parameters.Add(pm);
            }
        }
    }
}
