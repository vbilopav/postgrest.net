using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Npgsql;
using System.Linq;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Options;

namespace PostgRest.net
{
    public class ReferencValueType
    {
        private object value = DBNull.Value;
        public object Value { get => value; set => this.value = value ?? DBNull.Value; }
    }

    [Route("")]
    public abstract class ControllerBase<T> : ControllerBase
    {
        protected readonly IContentService contentService;
        private readonly IOptions<PostgRestConfig> options;
        private JObject query;
        private JObject body;
        private ControllerInfo info;
        private readonly IList<string> stringParameters;
        private readonly IList<NpgsqlParameter> npngParameters;

        protected ControllerBase(IContentService contentService, IOptions<PostgRestConfig> options)
        {
            this.contentService = contentService;
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
            if (!ControllerData.Data.TryGetValue(genericType.Name, out var info))
            {
                return null;
            }
            return info;
        }

        protected async Task<ContentResult> GetContentAsync()
        {
            info = GetInfo();
            if (info == null)
            {
                return new ContentResult { StatusCode = 400 };
            }
            info.Options.SetResponseParameters(info, contentService);
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

        private async Task<ContentResult> Execute() => 
            await contentService.GetContentAsync(
                $"{SelectExpression} {info.RoutineName}({string.Join(", ", stringParameters)})",
                parameters => parameters.AddRange(npngParameters.ToArray()), 
                IsRecordSet);

        private bool IsRecordSet => options.Value.RecordSetTypes.Contains(info.ReturnType);
        private string SelectExpression => IsRecordSet ? "select * from" : "select";

        private void ParseParamFromQueryString(Parameter param)
        {
            if (query == null)
            {
                query = Request.Query.ToJObject();
            }
            var value = new ReferencValueType { Value = query };
            info.Options.ApplyParameterValue?.Invoke(value, param.ParamName, info, this);
            npngParameters.Add(new NpgsqlParameter(param.ParamName, (value.Value as JObject).ToString(Formatting.None)));
        }

        private async Task ParseParamFromBody(Parameter param)
        {
            if (body == null)
            {
                body = JObject.Parse(await Request.GetBodyAsync());
            }
            var value = new ReferencValueType { Value = query };
            info.Options.ApplyParameterValue?.Invoke(value, param.ParamName, info, this);
            npngParameters.Add(new NpgsqlParameter(param.ParamName, (value.Value as JObject).ToString(Formatting.None)));
        }

        private void ParseCustomParam(Parameter param)
        {
            var value = new ReferencValueType();
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
                var value = new ReferencValueType { Value = string.Join("", Request.Query[param.ParamName].ToArray()) };
                info.Options.ApplyParameterValue?.Invoke(value, param.ParamName, info, this);
                npngParameters.Add(new NpgsqlParameter(param.ParamName, value.Value));
            }
        }
    }

    public class GetController<T> : ControllerBase<T>
    {
        public GetController(IContentService contentService, IOptions<PostgRestConfig> options) : base(contentService, options) { }

        [HttpGet]
        public async Task<ContentResult> Get() => await GetContentAsync();
    }

    public class PostController<T> : ControllerBase<T>
    {
        public PostController(IContentService contentService, IOptions<PostgRestConfig> options) : base(contentService, options) { }

        [HttpPost]
        public async Task<ContentResult> Post() => await GetContentAsync();
    }

    public class PutController<T> : ControllerBase<T>
    {
        public PutController(IContentService contentService, IOptions<PostgRestConfig> options) : base(contentService, options) { }

        [HttpPut]
        public async Task<ContentResult> Put() => await GetContentAsync();
    }

    public class DeleteController<T> : ControllerBase<T>
    {
        public DeleteController(IContentService contentService, IOptions<PostgRestConfig> options) : base(contentService, options) { }

        [HttpDelete]
        public async Task<ContentResult> Delete() => await GetContentAsync();
    }
}
