using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

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
            var parameters = "";
            var command = $"select {info.RoutineName}({parameters})";
            return await contentService.GetContentAsync(command);
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
