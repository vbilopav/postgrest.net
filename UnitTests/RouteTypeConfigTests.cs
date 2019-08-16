using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using PostgRest.net;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using static UnitTests.Config;

namespace UnitTests
{
    public class RouteTypeConfigTests : PostgRestClassFixture<RouteTypeConfigTests.RouteTypeConfig, RouteTypeConfigTests.LifeCycle>
    {
        public class RouteTypeConfig : IConfigureServices
        {
            public void ConfigureServices(IServiceCollection services) =>
                services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddPostgRest(services, new PostgRestOptions
                    {
                        Connection = TestingConnection,
                        IsGetRouteWhen = (route, _) => {
                            return route.EndsWith("get");
                         },
                        IsPostRouteWhen = (route, _) => route.EndsWith("post"),
                        IsPutRouteWhen = (route, _) => route.EndsWith("put"),
                        IsDeleteRouteWhen = (route, _) => route.EndsWith("delete")
                    });
        }

        public class LifeCycle : ILifeCycle
        {
            public void BuildUp() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"

            create function rest__get_route() returns void as $$ begin end $$ language plpgsql;
            create function rest__route_is_get() returns void as $$ begin end $$ language plpgsql;

            create function rest__post_route() returns void as $$ begin end $$ language plpgsql;
            create function rest__route_is_post() returns void as $$ begin end $$ language plpgsql;

            create function rest__put_route() returns void as $$ begin end $$ language plpgsql;
            create function rest__route_is_put() returns void as $$ begin end $$ language plpgsql;

            create function rest__delete_route() returns void as $$ begin end $$ language plpgsql;
            create function rest__route_is_delete() returns void as $$ begin end $$ language plpgsql;

            ");

            public void TearDown() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"

            drop function rest__get_route();
            drop function rest__route_is_get();

            drop function rest__post_route();
            drop function rest__route_is_post();

            drop function rest__put_route();
            drop function rest__route_is_put();

            drop function rest__delete_route();
            drop function rest__route_is_delete();

            ");

        }

        public RouteTypeConfigTests(
            ITestOutputHelper output,
            AspNetCoreFixture<RouteTypeConfig, LifeCycle> fixture) : base(output, fixture) {}


        [Fact]
        public async Task VerifyRouteIsGet()
        {
            var (_, status, _) = await RestClient.GetAsync<string>("https://localhost:5001/api/route-is-get");
            Assert.Equal(HttpStatusCode.NoContent, status);
        }

        [Fact]
        public async Task VerifyRouteIsNotGet()
        {
            var (_, status, _) = await RestClient.GetAsync<string>("https://localhost:5001/api/get-route");
            Assert.Equal(HttpStatusCode.NotFound, status);
            (_, status, _) = await RestClient.GetAsync<string>("https://localhost:5001/api/route");
            Assert.Equal(HttpStatusCode.NotFound, status);
        }

        [Fact]
        public async Task VerifyRouteIsPost()
        {
            var (_, status, _) = await RestClient.PostAsync<string>("https://localhost:5001/api/route-is-post", new StringContent(""));
            Assert.Equal(HttpStatusCode.NoContent, status);
        }

        [Fact]
        public async Task VerifyRouteIsNotPost()
        {
            var (_, status, _) = await RestClient.PostAsync<string>("https://localhost:5001/api/post-route", new StringContent(""));
            Assert.Equal(HttpStatusCode.NotFound, status);
            (_, status, _) = await RestClient.PostAsync<string>("https://localhost:5001/api/route", new StringContent(""));
            Assert.Equal(HttpStatusCode.NotFound, status);
        }

        [Fact]
        public async Task VerifyRouteIsPut()
        {
            var (_, status, _) = await RestClient.PutAsync<string>("https://localhost:5001/api/route-is-put", new StringContent(""));
            Assert.Equal(HttpStatusCode.NoContent, status);
        }

        [Fact]
        public async Task VerifyRouteIsNotPut()
        {
            var (_, status, _) = await RestClient.PutAsync<string>("https://localhost:5001/api/put-route", new StringContent(""));
            Assert.Equal(HttpStatusCode.NotFound, status);
            (_, status, _) = await RestClient.PutAsync<string>("https://localhost:5001/api/route", new StringContent(""));
            Assert.Equal(HttpStatusCode.NotFound, status);
        }

        [Fact]
        public async Task VerifyRouteIsDelete()
        {
            var (_, status, _) = await RestClient.DeleteAsync<string>("https://localhost:5001/api/route-is-delete");
            Assert.Equal(HttpStatusCode.NoContent, status);
        }

        [Fact]
        public async Task VerifyRouteIsNotDelete()
        {
            var (_, status, _) = await RestClient.DeleteAsync<string>("https://localhost:5001/api/delete-route");
            Assert.Equal(HttpStatusCode.NotFound, status);
            (_, status, _) = await RestClient.DeleteAsync<string>("https://localhost:5001/api/route");
            Assert.Equal(HttpStatusCode.NotFound, status);
        }
    }
}
