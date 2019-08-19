using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using static UnitTests.Config;

namespace UnitTests
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
            var (_, status, _) = await RestClient.GetAsync<string>("https://localhost:5001/api/route-is-get");
            Assert.Equal(HttpStatusCode.NoContent, status);
        }

        [Fact]
        public async Task VerifyRouteIsNotGet()
        {
            var (_, status, _) = await RestClient.GetAsync<string>("https://localhost:5001/api/route-is-not-get");
            Assert.Equal(HttpStatusCode.NotFound, status);
        }

        [Fact]
        public async Task VerifyRouteIsPost()
        {
            using (var content = new StringContent(""))
            {
                var (_, status, _) = await RestClient.PostAsync<string>("https://localhost:5001/api/route-is-post", content);
                Assert.Equal(HttpStatusCode.NoContent, status);
            }
        }

        [Fact]
        public async Task VerifyRouteIsNotPost()
        {
            using (var content = new StringContent(""))
            {
                var (_, status, _) = await RestClient.PostAsync<string>("https://localhost:5001/api/route-is-not-post", content);
                Assert.Equal(HttpStatusCode.NotFound, status);
            }
        }

        [Fact]
        public async Task VerifyRouteIsPut()
        {
            using (var content = new StringContent(""))
            {
                var (_, status, _) = await RestClient.PutAsync<string>("https://localhost:5001/api/route-is-put", content);
                Assert.Equal(HttpStatusCode.NoContent, status);
            }
        }

        [Fact]
        public async Task VerifyRouteIsNotPut()
        {
            using (var content = new StringContent(""))
            {
                var (_, status, _) = await RestClient.PutAsync<string>("https://localhost:5001/api/route-is-not-put", content);
                Assert.Equal(HttpStatusCode.NotFound, status);
            }
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
            var (_, status, _) = await RestClient.DeleteAsync<string>("https://localhost:5001/api/route-is-not-delete");
            Assert.Equal(HttpStatusCode.NotFound, status);
        }
    }
}
