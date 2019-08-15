using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using static UnitTests.Config;

namespace UnitTests
{
    public class LeastPriviledgeTests : PostgRestClassFixture<DefaultConfig, LeastPriviledgeTests.LifeCycle>
    {
        public class LifeCycle : ILifeCycle
        {
            public void BuildUp() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"

            create function rest__get_values_no_grant() returns json as
            $$
            begin
                return (
                    select json_build_object('values', array[1, 2, 3])
                );
            end
            $$ language plpgsql security definer;
            revoke all on function rest__get_values_no_grant() from public;

            create function rest__get_values_with_grant() returns json as
            $$
            begin
                return (
                    select json_build_object('values', array[1, 2, 3])
                );
            end
            $$ language plpgsql security definer;
            revoke all on function rest__get_values_with_grant() from public;

            grant execute on function rest__get_values_with_grant() to testing;

            ");

            public void TearDown() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"

            drop function rest__get_values_no_grant();
            drop function rest__get_values_with_grant();

            ");

        }

        public LeastPriviledgeTests(
            ITestOutputHelper output,
            AspNetCoreFixture<DefaultConfig, LifeCycle> fixture) : base(output, fixture) {}

        [Fact]
        public async Task VerifyNotFoundForEndpointWithNoGrant()
        {
            var (_, status, _) = await RestClient.GetAsync<object>("https://localhost:5001/api/values-no-grant");
            Assert.Equal(HttpStatusCode.NotFound, status);
        }

        public class ResponseModel
        {
            public int[] Values { get; set; }
        }

        [Fact]
        public async Task VerifyResponseForEndpointWithGrant()
        {
            var (response, status, contentType) = await RestClient.GetAsync<ResponseModel>("https://localhost:5001/api/values-with-grant");
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("application/json; charset=utf-8", contentType);
            Assert.Equal(new int[3] { 1,2,3 }, response.Values);
        }
    }
}
