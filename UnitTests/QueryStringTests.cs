using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using static UnitTests.Config;

namespace UnitTests
{
    public class QueryStringTests : PostgRestClassFixture<QueryStringTests.LifeCycle>
    {
        public class LifeCycle : ILifeCycle
        {
            public void BuildUp() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"
            create function rest__get_return_query(_query json) returns json as $$
            begin
                return _query;
            end $$ language plpgsql;

            create function rest__get_return_query_additional(_query json, _additional text) returns json as $$
            begin
                return _query::jsonb || format('{""additional"": ""%s""}', _additional)::jsonb;
            end $$ language plpgsql;
            ");

            public void TearDown() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"
            drop function rest__get_return_query(json);
            drop function rest__get_return_query_additional(json, text);
            ");
        }

        public QueryStringTests(
            ITestOutputHelper output,
            AspNetCoreFixture<LifeCycle> fixture) : base(output, fixture) {}

        [Fact]
        public async Task VerifyReturnQueryString()
        {
            var (result, _, _) = await RestClient.GetAsync<string>("https://localhost:5001/api/return-query?key1=value1&key2=value2&foo=bar&1=2");
            Assert.Equal(@"{""key1"":""value1"",""key2"":""value2"",""foo"":""bar"",""1"":""2""}", result);
        }

        [Fact]
        public async Task VerifyReturnQueryStringAdditionalParam()
        {
            var (result, _, _) = await RestClient.GetAsync<string>("https://localhost:5001/api/return-query-additional?foo=bar");
            Assert.Equal(@"{""foo"": ""bar"", ""additional"": """"}", result);
        }
    }
}
