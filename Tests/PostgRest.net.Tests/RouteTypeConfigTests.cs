using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using PostgRest.Net.Config;
using PostgRest.Net.ServiceConfig;
using PostgRest.net.Tests.TestingUtils;
using VerySimpleRestClient;
using Xunit;
using Xunit.Abstractions;
using static PostgRest.net.Tests.TestingUtils.Config;

namespace PostgRest.net.Tests
{
    public class RouteTypeConfigTests : PostgRestClassFixture<RouteTypeConfigTests.Services, RouteTypeConfigTests.LifeCycle>
    {
        public class Services : IConfigureServices
        {
            public void ConfigureServices(IServiceCollection services) =>
                services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddPostgRest(new PostgRestOptions
                    {
                        Connection = TestingConnection,
                        IsGetRouteWhen = (route, _) => route.EndsWith("get"),
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
            AspNetCoreFixture<Services, LifeCycle> fixture) : base(output, fixture) {}


        [Fact]
        public async Task VerifyRouteIsGet()
        {
            var (_, response) = await Client.GetAsync("https://localhost:5001/api/route-is-get");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task VerifyRouteIsNotGet()
        {
            using (var client = new HttpClient())
            {
                var (_, response) = await Client.GetAsync("https://localhost:5001/api/get-route", client: client);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                (_, response) = await Client.GetAsync("https://localhost:5001/api/route", client: client);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task VerifyRouteIsPost()
        {
            var (_, response) = await Client.PostAsync("https://localhost:5001/api/route-is-post", body: new TextPlain(""));
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task VerifyRouteIsNotPost()
        {
            using (var client = new HttpClient())
            {
                var (_, response) = await Client.PostAsync("https://localhost:5001/api/post-route", client: client);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                (_, response) = await Client.PostAsync("https://localhost:5001/api/route", client: client);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task VerifyRouteIsPut()
        {
            var (_, response) = await Client.PutAsync("https://localhost:5001/api/route-is-put", body: new TextPlain(""));
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task VerifyRouteIsNotPut()
        {
            using (var client = new HttpClient())
            {
                var (_, response) = await Client.PutAsync("https://localhost:5001/api/put-route", client: client);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                (_, response) = await Client.PutAsync("https://localhost:5001/api/route", client: client);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task VerifyRouteIsDelete()
        {
            var (_, response) = await Client.DeleteAsync<string>("https://localhost:5001/api/route-is-delete");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task VerifyRouteIsNotDelete()
        {
            using (var client = new HttpClient())
            {
                var (_, response) = await Client.DeleteAsync<string>("https://localhost:5001/api/delete-route", client: client);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                (_, response) = await Client.DeleteAsync<string>("https://localhost:5001/api/route", client: client);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
    }
}
