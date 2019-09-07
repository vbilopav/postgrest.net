using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using PostgRest.Net.Config;
using PostgRest.Net.DataServices;
using PostgRest.Net.ServiceConfig;

namespace PostgRest.Net.Controllers
{
    [Route("")]
    public abstract class ControllerBase<T> : ControllerBase
    {
        protected readonly IStringContentService StringContentService;

        private readonly IOptions<PostgRestConfig> options;
        private JObject query;
        private object body;
        private ControllerInfo info;
        private readonly IList<string> stringParameters;
        private readonly IList<NpgsqlParameter> npngParameters;

        protected ControllerBase(IStringContentService stringContentService, IOptions<PostgRestConfig> options)
        {
            this.StringContentService = stringContentService;
            this.options = options;
            query = null;
            body = null;
            info = null;
            stringParameters = new List<string>();
            npngParameters = new List<NpgsqlParameter>();
        }

        internal ControllerInfo GetInfo()
        {
            var genericType = this.GetType().GenericTypeArguments[0];
            return !ControllerData.Data.TryGetValue(genericType.Name, out var controllerInfo) ? null : controllerInfo;
        }

        protected async Task<ContentResult> GetContentAsync()
        {
            info = GetInfo();
            if (info == null)
            {
                return new ContentResult { StatusCode = 400 };
            }
            info.Options.SetResponseParameters(info, StringContentService);
            if (info.Parameters.Count == 0)
            {
                return await Execute();
            }

            foreach (var param in info.Parameters)
            {
                if (param.Direction == "OUT")
                {
                    continue;
                }

                if (info.MatchParamsByQueryStringKey)
                {
                    ParseMatchParamsByQueryStringKeyName(param);
                }
                else if (info.MatchParamsByFormKey)
                {
                    ParseMatchParamsByBFormKeyName(param);
                }
                else if (param.FromQueryString)
                {
                    ParseParamFromQueryString(param);
                }
                else if (param.FromBody)
                {
                    await ParseParamFromBody(param);
                }
                else
                {
                    ParseCustomParam(param);
                }
                stringParameters.Add($"@{param.ParamName}::{param.ParamType}");
            }

            return await Execute();
        }

        private async Task<ContentResult> Execute()
        {
            var isRecordSet = options.Value.RecordSetTypes.Contains(info.ReturnType);
            var expression = isRecordSet ? "select * from" : "select";
            return await StringContentService.GetContentAsync(
                $"{expression} {info.RoutineName}({string.Join(", ", stringParameters)})",
                parameters => parameters.AddRange(npngParameters.ToArray()),
                isRecordSet);
        }

        private void ParseParamFromQueryString(Parameter param)
        {
            if (query == null)
            {
                query = Request.Query.ToJObject();
            }
            var value = new ReferenceValueType { Value = query };
            info.Options.ApplyParameterValue?.Invoke(value, param.ParamName, info, this);
            npngParameters.Add(new NpgsqlParameter(param.ParamName, ((JObject) value.Value).ToString(Formatting.None)));
        }

        private async Task ParseParamFromBody(Parameter param)
        {
            if (body == null)
            {
                var content = await Request.GetBodyAsync();
                if (Request.ContentType.StartsWith("application/json"))
                {
                    if (!string.IsNullOrEmpty(content))
                    {
                        body = JObject.Parse(content);
                    }
                }
                else if (Request.ContentType.StartsWith("multipart/form-data"))
                {
                    body = Request.Form.ToJObject();
                }
                else
                {
                    body = content;
                }
            }
            var value = new ReferenceValueType { Value = body };
            info.Options.ApplyParameterValue?.Invoke(value, param.ParamName, info, this);
            var actual = value.Value is JObject jObject ? jObject.ToString(Formatting.None) : value.Value;
            npngParameters.Add(new NpgsqlParameter(param.ParamName, actual));
        }

        private void ParseCustomParam(Parameter param)
        {
            var value = new ReferenceValueType();
            var routeValue = this.ControllerContext.RouteData?.Values?[param.ParamName];
            if (routeValue != null)
            {
                value.Value = routeValue;
            }
            info.Options.ApplyParameterValue?.Invoke(value, param.ParamName, info, this);
            npngParameters.Add(new NpgsqlParameter(param.ParamName, value.Value));
        }

        private void ParseMatchParamsByQueryStringKeyName(Parameter param)
        {
            if (Request.Query[param.ParamName] == StringValues.Empty)
            {
                ParseCustomParam(param);
            }
            else
            {
                var value = new ReferenceValueType { Value = string.Join("", Request.Query[param.ParamName].ToArray()) };
                info.Options.ApplyParameterValue?.Invoke(value, param.ParamName, info, this);
                npngParameters.Add(new NpgsqlParameter(param.ParamName, value.Value));
            }
        }

        private void ParseMatchParamsByBFormKeyName(Parameter param)
        {
            if (Request.Form[param.ParamName] == StringValues.Empty)
            {
                ParseCustomParam(param);
            }
            else
            {
                var value = new ReferenceValueType { Value = string.Join("", Request.Form[param.ParamName].ToArray()) };
                info.Options.ApplyParameterValue?.Invoke(value, param.ParamName, info, this);
                npngParameters.Add(new NpgsqlParameter(param.ParamName, value.Value));
            }
        }
    }
}