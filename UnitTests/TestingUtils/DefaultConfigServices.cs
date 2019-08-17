using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using PostgRest.net;
using static UnitTests.Config;

namespace UnitTests
{
    public class DefaultConfigServices : IConfigureServices
    {
        public void ConfigureServices(IServiceCollection services) =>
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddPostgRest(services, new PostgRestOptions
                {
                    Connection = TestingConnection
                });
    }
}
