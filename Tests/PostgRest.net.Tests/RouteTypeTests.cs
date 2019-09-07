using System.Net;
using System.Threading.Tasks;
using PostgRest.net.Tests.TestingUtils;
using VerySimpleRestClient;
using Xunit;
using Xunit.Abstractions;
using static PostgRest.net.Tests.TestingUtils.Config;

namespace PostgRest.net.Tests
{
    public class RouteTypeTests : PostgRestClassFixture<RouteTypeTests.LifeCycle>
    {
        public class LifeCycle : ILifeCycle
        {
            public void BuildUp() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"

            create function rest__get_route_is_get() returns void as $$ begin end $$ language plpgsql;
            create function rest__route_is_not_get() returns void as $$ begin end $$ language plpgsql;

            create function rest__post_route_is_post() returns void as $$ begin end $$ language plpgsql;
            create function rest__route_is_not_post() returns void as $$ begin end $$ language plpgsql;

            create function rest__put_route_is_put() returns void as $$ begin end $$ language plpgsql;
            create function rest__route_is_not_put() returns void as $$ begin end $$ language plpgsql;

            create function rest__delete_route_is_delete() returns void as $$ begin end $$ language plpgsql;
            create function rest__route_is_not_delete() returns void as $$ begin end $$ language plpgsql;

            ");

            public void TearDown() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"

            drop function rest__get_route_is_get();
            drop function rest__route_is_not_get();

            drop function rest__post_route_is_post();
            drop function rest__route_is_not_post();

            drop function rest__put_route_is_put();
            drop function rest__route_is_not_put();

            drop function rest__delete_route_is_delete();
            drop function rest__route_is_not_delete();

            ");

        }

        public RouteTypeTests(
            ITestOutputHelper output,
            AspNetCoreFixture<LifeCycle> fixture) : base(output, fixture) {}


        [Fact]
        public async Task VerifyRouteIsGet()
        {
            var (_, response) = await Client.GetAsync("https://localhost:5001/api/route-is-get");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task VerifyRouteIsNotGet()
        {
            var (_, response) = await Client.GetAsync("https://localhost:5001/api/route-is-not-get");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task VerifyRouteIsPost()
        {

            var (_, response) = await Client.PostAsync<string>("https://localhost:5001/api/route-is-post");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task VerifyRouteIsNotPost()
        {
            var (_, response) = await Client.PostAsync<string>("https://localhost:5001/api/route-is-not-post");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task VerifyRouteIsPut()
        {
            var (_, response) = await Client.PutAsync<string>("https://localhost:5001/api/route-is-put");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task VerifyRouteIsNotPut()
        {
            var (_, response) = await Client.PutAsync<string>("https://localhost:5001/api/route-is-not-put");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
            var (_, response) = await Client.DeleteAsync<string>("https://localhost:5001/api/route-is-not-delete");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
