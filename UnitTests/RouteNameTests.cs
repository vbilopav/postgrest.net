using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using PostgRest.net;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using static UnitTests.Config;

namespace UnitTests
{
    public class RouteNameTests : PostgRestClassFixture<RouteNameTests.Services, RouteNameTests.LifeCycle>
    {
        public class TestRouteResolver : IRouteNameResolver
        {
            public string ResolveRouteName(string routineName, string candidateRaw, string candidateLowerNoVerb, string verb) => routineName;
        }

        public class Services : IConfigureServices
        {
            public void ConfigureServices(IServiceCollection services) =>
                services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddPostgRest(services, new PostgRestOptions
                    {
                        Connection = TestingConnection,
                        RouteNameResolver = new TestRouteResolver(),
                        RouteNamePattern = "api/v2/{0}",
                        ApplyRouteName = (route, routine) => $"{route}-test"
                    });
        }

        public class LifeCycle : ILifeCycle
        {
            public void BuildUp() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"

            create function rest__get_test_route_name() returns json as
            $$
            begin
                return '{}';
            end
            $$ language plpgsql;

            ");

            public void TearDown() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"

            drop function rest__get_test_route_name();

            ");

        }

        public RouteNameTests(
            ITestOutputHelper output,
            AspNetCoreFixture<Services, LifeCycle> fixture) : base(output, fixture) { }

        [Fact]
        public async Task VerifyEndpintWithNewRouteName()
        {
            var (_, status, _) = await RestClient.GetAsync<object>("https://localhost:5001/api/v2/rest__get_test_route_name-test");
            Assert.Equal(HttpStatusCode.OK, status);
        }
    }
}