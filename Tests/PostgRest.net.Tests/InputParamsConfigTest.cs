using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using PostgRest.Net.Config;
using PostgRest.Net.ServiceConfig;
using PostgRest.net.Tests.TestingUtils;
using VerySimpleRestClient;
using Xunit;
using Xunit.Abstractions;
using static PostgRest.net.Tests.TestingUtils.Config;

namespace PostgRest.net.Tests
{
    public class InputParamsConfigTest : PostgRestClassFixture<InputParamsConfigTest.Services, InputParamsConfigTest.LifeCycle>
    {
        public class Services : IConfigureServices
        {
            public void ConfigureServices(IServiceCollection services) =>
                services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddPostgRest(new PostgRestOptions
                    {
                        Connection = TestingConnection,
                        ApplyParameterValue = (value, name, info, controller) =>
                        {
                            switch (info.RouteName)
                            {
                                case "api/return-query-additional-applied/{_additional}/" when name == "_additional":
                                    value.Value = "some text";
                                    break;
                                case "api/return-query-applied/" when name == "_query":
                                    ((JObject)value.Value)["foo"] = "bar";
                                    break;
                            }
                        }
                    });
        }

        public class LifeCycle : ILifeCycle
        {
            public void BuildUp() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"
            create function rest__get_return_query_applied(_query json) returns json as $$
            begin
                return _query;
            end $$ language plpgsql;

            create function rest__get_return_query_additional_applied(_query json, _additional text) returns json as $$
            begin
                return _query::jsonb || format('{""additional"": ""%s""}', _additional)::jsonb;
            end $$ language plpgsql;

            create function rest__delete_return_query_applied(_query json) returns json as $$
            begin
                return _query;
            end $$ language plpgsql;

            create function rest__delete_return_query_additional_applied(_query json, _additional text) returns json as $$
            begin
                return _query::jsonb || format('{""additional"": ""%s""}', _additional)::jsonb;
            end $$ language plpgsql;
            ");

            public void TearDown() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"
            drop function rest__get_return_query_applied(json);
            drop function rest__get_return_query_additional_applied(json, text);
            drop function rest__delete_return_query_applied(json);
            drop function rest__delete_return_query_additional_applied(json, text);
            ");
        }

        public InputParamsConfigTest(
            ITestOutputHelper output,
            AspNetCoreFixture<Services, LifeCycle> fixture) : base(output, fixture) {}

        [Fact]
        public async Task VerifyGetAdditionalValuesResults()
        {
            var result = await SimpleClient.GetAsync("https://localhost:5001/api/return-query-applied", new Query(new { key1 = "value1"}));

            Assert.Equal("value1", result["key1"]);
            Assert.Equal("bar", result["foo"]);
        }

        [Fact]
        public async Task VerifyGetReturnQueryStringAdditionalParam()
        {
            var result = await SimpleClient.GetAsync("https://localhost:5001/api/return-query-additional-applied/path-param/", new Query(new {foo = "bar"}));
            Assert.Equal("some text", result["additional"]);
            Assert.Equal("bar", result["foo"]);
        }

        [Fact]
        public async Task VerifyDeleteAdditionalValuesResults()
        {
            var result = await SimpleClient.DeleteAsync("https://localhost:5001/api/return-query-applied", new Query(new { key1 = "value1" }));
            Assert.Equal("value1", result["key1"]);
            Assert.Equal("bar", result["foo"]);
        }

        [Fact]
        public async Task VerifyDeleteReturnQueryStringAdditionalParam()
        {
            var result = await SimpleClient.DeleteAsync("https://localhost:5001/api/return-query-additional-applied/path-param/", new Query(new { foo = "bar" }));
            Assert.Equal("some text", result["additional"]);
            Assert.Equal("bar", result["foo"]);
        }
    }
}
