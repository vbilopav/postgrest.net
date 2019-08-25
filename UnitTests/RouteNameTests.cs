using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using PostgRest.net;
using System.Net;
using System.Threading.Tasks;
using VerySimpleRestClient;
using Xunit;
using Xunit.Abstractions;
using static UnitTests.Config;

namespace UnitTests
{
    public class RouteNameTests : PostgRestClassFixture<RouteNameTests.Services, RouteNameTests.LifeCycle>
    {
        public class Services : IConfigureServices
        {
            public void ConfigureServices(IServiceCollection services) =>
                services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddPostgRest(new PostgRestOptions
                    {
                        Connection = TestingConnection,
                        ResolveRouteName = (routine, routineNoPrefix, routineLowerNoPrefix, verb) => routine,
                        RouteNamePattern = "api/v2/{0}",
                        ApplyRouteName = (route, routine) => $"{route}-test"
                    });
        }

        public class LifeCycle : ILifeCycle
        {
            public void BuildUp() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"

            create function rest__get_test_route_name() returns json as $$ begin return '{}'; end $$ language plpgsql;
            create function rest__post_test_route_name() returns json as $$ begin return '{}'; end $$ language plpgsql;
            create function rest__put_test_route_name() returns json as $$ begin return '{}'; end $$ language plpgsql;
            create function rest__delete_test_route_name() returns json as $$ begin return '{}'; end $$ language plpgsql;

            ");

            public void TearDown() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"

            drop function rest__get_test_route_name();
            drop function rest__post_test_route_name();
            drop function rest__put_test_route_name();
            drop function rest__delete_test_route_name();

            ");

        }

        public RouteNameTests(
            ITestOutputHelper output,
            AspNetCoreFixture<Services, LifeCycle> fixture) : base(output, fixture) { }

        [Fact]
        public async Task VerifyEndpointWithNewRouteNameForGet()
        {
            var (_, response) = await Client.GetAsync<object>("https://localhost:5001/api/v2/rest__get_test_route_name-test");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task VerifyEndpointWithNewRouteNameForPost()
        {
            var (_, response) = await Client.PostAsync<object>("https://localhost:5001/api/v2/rest__post_test_route_name-test");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task VerifyEndpointWithNewRouteNameForPut()
        {
            var (_, response) = await Client.PutAsync<object>("https://localhost:5001/api/v2/rest__put_test_route_name-test");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task VerifyEndpointWithNewRouteNameForDelete()
        {
            var (_, response) = await Client.DeleteAsync<object>("https://localhost:5001/api/v2/rest__delete_test_route_name-test");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
