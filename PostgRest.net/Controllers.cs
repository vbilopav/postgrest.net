using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Npgsql;

namespace PostgRest.net
{
    [Route("")]
    public abstract class ControllerBase<T> : ControllerBase
    {
        protected readonly IContentService contentService;

        protected ControllerBase(IContentService contentService)
        {
            this.contentService = contentService;
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
            var info = GetInfo();
            if (info == null)
            {
                return new ContentResult { StatusCode = 400 };
            }
            info.Options.SetResponseParameters(info, contentService);
            if (info.Parameters.Count == 0)
            {
                return await contentService.GetContentAsync($"select {info.RoutineName}()");
            }
            JObject query = null;
            JObject body = null;
            var stringParameters = new List<string>();
            var npngParameters = new List<NpgsqlParameter>();
            foreach(var param in info.Parameters)
            {
                /*
                if (info.MatchParamsByQueryStringKeyName)
                {
                    var value = Request.Query[param.ParamName];
                }
                else 
                */
                if (param.FromQueryString)
                {
                    if (query == null)
                    {
                        query = Request.Query.ToJObject();
                    }
                    var value = new ReferencValueType { Value = query };
                    info.Options.ApplyParameterValue?.Invoke(value, param.ParamName, info, this);
                    npngParameters.Add(new NpgsqlParameter(param.ParamName, (value.Value as JObject).ToString(Formatting.None)));
                }

                else if (param.FromBody)
                {
                    if (body == null)
                    {
                        body = JObject.Parse(await Request.GetBodyAsync());
                    }
                    npngParameters.Add(new NpgsqlParameter(param.ParamName, body.ToString(Formatting.None)));
                }

                else
                {
                    var value = new ReferencValueType { Value = DBNull.Value };
                    info.Options.ApplyParameterValue?.Invoke(value, param.ParamName, info, this);
                    npngParameters.Add(new NpgsqlParameter(param.ParamName, value.Value));
                }
                stringParameters.Add($"@{param.ParamName}::{param.ParamType}");
            }
            var command = $"select {info.RoutineName}({string.Join(", ", stringParameters)})";
            return await contentService.GetContentAsync(command, parameters => parameters.AddRange(npngParameters.ToArray()));
        }
    }

    public class GetController<T> : ControllerBase<T>
    {
        public GetController(IContentService contentService) : base(contentService) { }

        [HttpGet]
        public async Task<ContentResult> Get() => await GetContentAsync();
    }

    public class PostController<T> : ControllerBase<T>
    {
        public PostController(IContentService contentService) : base(contentService) { }

        [HttpPost]
        public async Task<ContentResult> Post() => await GetContentAsync();
    }

    public class PutController<T> : ControllerBase<T>
    {
        public PutController(IContentService contentService) : base(contentService) { }

        [HttpPut]
        public async Task<ContentResult> Put() => await GetContentAsync();
    }

    public class DeleteController<T> : ControllerBase<T>
    {
        public DeleteController(IContentService contentService) : base(contentService) { }

        [HttpDelete]
        public async Task<ContentResult> Delete() => await GetContentAsync();
    }
}
