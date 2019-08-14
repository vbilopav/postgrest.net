using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using PostgRest.net;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests
{
    public class TestingConnectionConfig : IConfigureServices
    {
        public void ConfigureServices(IServiceCollection services) =>
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddPostgRest(services, new PostgRestOptions
                {
                    Connection = Config.TestingConnection
                });
    }
}
