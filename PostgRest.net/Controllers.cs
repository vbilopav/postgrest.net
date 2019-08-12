using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace PostgRest.net
{
    public class PgGetController : ControllerBase
    {
        private readonly IServiceProvider provider;

        protected PgGetController() : base()
        {
            provider = null;
        }

        public PgGetController(IServiceProvider provider) : base()
        {
            this.provider = provider;
        }

        [HttpGet]
        public ContentResult Get()
        {
            //var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            if (!ControllerData.Data.TryGetValue(this.GetType().Name, out var info))
            {
                return new ContentResult { StatusCode = 400 };
            }

            return new ContentResult { Content = this.GetType().Name};
        }
    }

    public class PgPostController : ControllerBase
    {
        [HttpPost]
        public ContentResult Post()
        {
            return new ContentResult { Content = "test" };
        }
    }

    public class PgPutController : ControllerBase
    {
        [HttpPut]
        public ContentResult Put()
        {
            return new ContentResult { Content = "test" };
        }
    }

    public class PgDeleteController : ControllerBase
    {
        [HttpDelete]
        public ContentResult Delete()
        {
            return new ContentResult { Content = "test" };
        }
    }
}
