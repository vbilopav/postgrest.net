using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using PostgRest.net;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using static UnitTests.Config;

namespace UnitTests
{
    public class InputParamsConfigTest : PostgRestClassFixture<InputParamsConfigTest.Services, InputParamsConfigTest.LifeCycle>
    {
        public class Services : IConfigureServices
        {
            public void ConfigureServices(IServiceCollection services) =>
                services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddPostgRest(services, new PostgRestOptions
                    {
                        Connection = TestingConnection,
                        ApplyParameterValue = (value, name, info, controller) =>
                        {
                            if (info.RoutineName == "rest__get_return_query_additional_applied" && name == "_additional")
                            {
                                value.Value = "some text";
                            }
                            else if (info.RoutineName == "rest__get_return_query_applied" && name == "_query")
                            {
                                (value.Value as JObject)["foo"] = "bar";
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
            ");

            public void TearDown() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"
            drop function rest__get_return_query_applied(json);
            drop function rest__get_return_query_additional_applied(json, text);
            ");
        }

        public InputParamsConfigTest(
            ITestOutputHelper output,
            AspNetCoreFixture<Services, LifeCycle> fixture) : base(output, fixture) {}

        [Fact]
        public async Task VerifyAdditionalValuesResults()
        {
            var (result, status, _) = await RestClient.GetAsync<string>("https://localhost:5001/api/return-query-applied?key1=value1");
            Assert.Equal(@"{""key1"":""value1"",""foo"":""bar""}", result);
        }

        [Fact]
        public async Task VerifyReturnQueryStringAdditionalParam()
        {
            var (result, _, _) = await RestClient.GetAsync<string>("https://localhost:5001/api/return-query-additional-applied?foo=bar");
            Assert.Equal(@"{""foo"": ""bar"", ""additional"": ""some text""}", result);
        }
    }
}
