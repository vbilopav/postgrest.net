using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;
using static UnitTests.Config;

namespace UnitTests
{
    public class QueryStringParamsTests : PostgRestClassFixture<QueryStringParamsTests.LifeCycle>
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

            create function rest__post_return_query(_query json) returns json as $$
            begin
                return _query;
            end $$ language plpgsql;

            create function rest__post_return_query_additional(_query json, _additional text) returns json as $$
            begin
                return _query::jsonb || format('{""additional"": ""%s""}', _additional)::jsonb;
            end $$ language plpgsql;

            create function rest__put_return_query(_query json) returns json as $$
            begin
                return _query;
            end $$ language plpgsql;

            create function rest__put_return_query_additional(_query json, _additional text) returns json as $$
            begin
                return _query::jsonb || format('{""additional"": ""%s""}', _additional)::jsonb;
            end $$ language plpgsql;

            create function rest__delete_return_query(_query json) returns json as $$
            begin
                return _query;
            end $$ language plpgsql;

            create function rest__delete_return_query_additional(_query json, _additional text) returns json as $$
            begin
                return _query::jsonb || format('{""additional"": ""%s""}', _additional)::jsonb;
            end $$ language plpgsql;

            ");

            public void TearDown() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"

            drop function rest__get_return_query(json);
            drop function rest__get_return_query_additional(json, text);

            drop function rest__post_return_query(json);
            drop function rest__post_return_query_additional(json, text);

            drop function rest__put_return_query(json);
            drop function rest__put_return_query_additional(json, text);

            drop function rest__delete_return_query(json);
            drop function rest__delete_return_query_additional(json, text);

            ");
        }

        public QueryStringParamsTests(
            ITestOutputHelper output,
            AspNetCoreFixture<LifeCycle> fixture) : base(output, fixture) {}

        [Fact]
        public async Task VerifyGetReturnQueryString()
        {
            var (result, _, _) = await RestClient.GetAsync<JObject>("https://localhost:5001/api/return-query?key1=value1&key2=value2&foo=bar&1=2");
            Assert.Equal("value1", result["key1"]);
            Assert.Equal("value2", result["key2"]);
            Assert.Equal("bar", result["foo"]);
            Assert.Equal("2", result["1"]);
        }

        [Fact]
        public async Task VerifyGetReturnQueryStringAdditionalParam()
        {
            var (result, _, _) = await RestClient.GetAsync<JObject>("https://localhost:5001/api/return-query-additional?foo=bar");
            Assert.Equal("bar", result["foo"]);
            Assert.Equal("", result["additional"]);
        }

        [Fact]
        public async Task VerifyPostReturnQueryString()
        {
            var (result, _, _) = await RestClient.PostAsync<JObject>("https://localhost:5001/api/return-query?key1=value1&key2=value2&foo=bar&1=2", null);
            Assert.Equal("value1", result["key1"]);
            Assert.Equal("value2", result["key2"]);
            Assert.Equal("bar", result["foo"]);
            Assert.Equal("2", result["1"]);
        }

        [Fact]
        public async Task VerifyPostReturnQueryStringAdditionalParam()
        {
            var (result, _, _) = await RestClient.PostAsync<JObject>("https://localhost:5001/api/return-query-additional?foo=bar", null);
            Assert.Equal("bar", result["foo"]);
            Assert.Equal("", result["additional"]);
        }

        [Fact]
        public async Task VerifyPutReturnQueryString()
        {
            var (result, _, _) = await RestClient.PutAsync<JObject>("https://localhost:5001/api/return-query?key1=value1&key2=value2&foo=bar&1=2", null);
            Assert.Equal("value1", result["key1"]);
            Assert.Equal("value2", result["key2"]);
            Assert.Equal("bar", result["foo"]);
            Assert.Equal("2", result["1"]);
        }

        [Fact]
        public async Task VerifyPutReturnQueryStringAdditionalParam()
        {
            var (result, _, _) = await RestClient.PutAsync<JObject>("https://localhost:5001/api/return-query-additional?foo=bar", null);
            Assert.Equal("bar", result["foo"]);
            Assert.Equal("", result["additional"]);
        }

        [Fact]
        public async Task VerifyDeleteReturnQueryString()
        {
            var (result, _, _) = await RestClient.DeleteAsync<JObject>("https://localhost:5001/api/return-query?key1=value1&key2=value2&foo=bar&1=2");
            Assert.Equal("value1", result["key1"]);
            Assert.Equal("value2", result["key2"]);
            Assert.Equal("bar", result["foo"]);
            Assert.Equal("2", result["1"]);
        }

        [Fact]
        public async Task VerifyDeleteReturnQueryStringAdditionalParam()
        {
            var (result, _, _) = await RestClient.DeleteAsync<JObject>("https://localhost:5001/api/return-query-additional?foo=bar");
            Assert.Equal("bar", result["foo"]);
            Assert.Equal("", result["additional"]);
        }
    }
}
