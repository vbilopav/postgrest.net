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
    public abstract class PgBaseController<T> : ControllerBase
    {
        protected readonly IPgDataContentService contentService;

        protected PgBaseController(IPgDataContentService contentService)
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
            if (info.ReturnType == "json" || info.ReturnType == "jsonb")
            {
                contentService.SetContentParameters(200, "application/json; charset=utf-8", "{}");
            } else if (info.ReturnType == "void")
            {
                contentService.SetContentParameters(204, "text/plain; charset=utf-8", null);
            } else
            {
                contentService.SetContentParameters(200, "text/plain; charset=utf-8", null);
            }
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
                if (param.FromQueryString)
                {
                    if (query == null)
                    {
                        query = Request.Query.ToJObject();
                    }
                    npngParameters.Add(new NpgsqlParameter(param.ParamName, query.ToString(Formatting.None)));
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
                    npngParameters.Add(new NpgsqlParameter(param.ParamName, null));
                }
                stringParameters.Add($"@{param.ParamName}::{param.ParamType}");
            }
            var command = $"select {info.RoutineName}({string.Join(", ", stringParameters)})";
            return await contentService.GetContentAsync(command, parameters => parameters.AddRange(npngParameters.ToArray()));
        }
    }

    public class PgGetController<T> : PgBaseController<T>
    {
        public PgGetController(IPgDataContentService contentService) : base(contentService) { }

        [HttpGet]
        public async Task<ContentResult> Get() => await GetContentAsync();
    }

    public class PgPostController<T> : PgBaseController<T>
    {
        public PgPostController(IPgDataContentService contentService) : base(contentService) { }

        [HttpPost]
        public async Task<ContentResult> Post() => await GetContentAsync();
    }

    public class PgPutController<T> : PgBaseController<T>
    {
        public PgPutController(IPgDataContentService contentService) : base(contentService) { }

        [HttpPut]
        public async Task<ContentResult> Put() => await GetContentAsync();
    }

    public class PgDeleteController<T> : PgBaseController<T>
    {
        public PgDeleteController(IPgDataContentService contentService) : base(contentService) { }

        [HttpDelete]
        public async Task<ContentResult> Delete() => await GetContentAsync();
    }
}
