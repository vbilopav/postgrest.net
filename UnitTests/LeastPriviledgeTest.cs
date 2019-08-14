using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{

    public class LeastPriviledgeTest : PostgRestClassFixture<
        TestingConnectionConfig,
        LeastPriviledgeTest.LifeCycle>
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
            using (var client = new HttpClient())
            using (var response = await client.GetAsync("https://localhost:5001/api/values-no-grant"))
            {
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task TryExecuteWithGrantAndVerify()
        {
            using (var client = new HttpClient())
            using (var response = await client.GetAsync("https://localhost:5001/api/values-with-grant", HttpCompletionOption.ResponseHeadersRead))
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("application/json", response.Content.Headers.ContentType.ToString());

                var result = JObject.Parse(await response.Content.ReadAsStringAsync());
                var list = result["values"].Children().ToList();
                Assert.Equal(new List<JToken> { JToken.Parse("1"), JToken.Parse("2"), JToken.Parse("3") }, list);
            }
        }
    }
}
