using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using PostgRest.net;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class ConfigureUnitTest1: IConfigureServices
    {
        public void ConfigureServices(IServiceCollection services) =>
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddPostgRest(services, new PostgRestOptions
                {
                    Connection = Config.TestingConnection
                });
    }

    public class UnitTest1 : PostgRestClassFixture<ConfigureUnitTest1>
    {
        public UnitTest1(
            ITestOutputHelper output,
            AspNetCoreFixture<ConfigureUnitTest1> fixture) : base(output, fixture)
        {
        }

        [Fact]
        public async Task Test1()
        {
            using (var client = new HttpClient())
            using (var response = await client.GetAsync("https://localhost:5001/api/values"))
            {
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
    }
}
