using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Microsoft.Extensions.Logging;

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

        protected ContentResult GetContent()
        {
            var info = GetInfo();
            if (info == null)
            {
                return new ContentResult { StatusCode = 400 };
            }

            return new ContentResult { Content = info.RoutineName };
        }
    }

    public class PgGetController<T> : PgBaseController<T>
    {
        public PgGetController(IPgDataContentService contentService) : base(contentService) { }

        [HttpGet]
        public ContentResult Get() => GetContent();
    }

    public class PgPostController<T> : PgBaseController<T>
    {
        public PgPostController(IPgDataContentService contentService) : base(contentService) { }

        [HttpPost]
        public ContentResult Post() => GetContent();
    }

    public class PgPutController<T> : PgBaseController<T>
    {
        public PgPutController(IPgDataContentService contentService) : base(contentService) { }

        [HttpPut]
        public ContentResult Put() => GetContent();
    }

    public class PgDeleteController<T> : PgBaseController<T>
    {
        public PgDeleteController(IPgDataContentService contentService) : base(contentService) { }

        [HttpDelete]
        public ContentResult Delete() => GetContent();
    }
}
