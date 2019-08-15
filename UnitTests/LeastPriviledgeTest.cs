using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class LeastPriviledgeTest :
        PostgRestClassFixture<TestingConnectionConfig, LeastPriviledgeTest.LifeCycle>
    {
        public class LifeCycle : ILifeCycle
        {
            public void BuildUp()
            {
                DatabaseFixture.ExecuteCommand(@"
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
            }
            public void TearDown()
            {
                DatabaseFixture.ExecuteCommand(@"
                drop function rest__get_values_no_grant();
                drop function rest__get_values_with_grant();
                ");
            }
        }

        public LeastPriviledgeTest(
            ITestOutputHelper output,
            AspNetCoreFixture<TestingConnectionConfig, LifeCycle> fixture) : base(output, fixture)
        {
        }

        [Fact]
        public async Task TryExecuteWithoutGrant()
        {
            var (_, status, _) = await RestClient.GetAsync<object>("https://localhost:5001/api/values-no-grant");
            Assert.Equal(HttpStatusCode.NotFound, status);
        }

        public class ResponseModel
        {
            public int[] Values { get; set; }
        }

        [Fact]
        public async Task TryExecuteWithGrantAndVerify()
        {
            var (response, status, contentType) = await RestClient.GetAsync<ResponseModel>("https://localhost:5001/api/values-with-grant");
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("application/json", contentType);
            Assert.Equal(new int[3] { 1,2,3 }, response.Values);
        }
    }
}
