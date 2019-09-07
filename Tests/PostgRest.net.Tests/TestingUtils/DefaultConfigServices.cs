using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using PostgRest.Net.Config;
using PostgRest.Net.ServiceConfig;
using static PostgRest.net.Tests.TestingUtils.Config;

namespace PostgRest.net.Tests.TestingUtils
{
    public class DefaultConfigServices : IConfigureServices
    {
        public void ConfigureServices(IServiceCollection services) =>
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddPostgRest(new PostgRestOptions
                {
                    Connection = TestingConnection
                });
    }
}
