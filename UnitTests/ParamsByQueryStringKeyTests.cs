using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using PostgRest.net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using static UnitTests.Config;

namespace UnitTests
{
    public class ParamsByQueryStringKeyTests : PostgRestClassFixture<ParamsByQueryStringKeyTests.Services, ParamsByQueryStringKeyTests.LifeCycle>
    {
        public class Services : IConfigureServices
        {
            public void ConfigureServices(IServiceCollection services) =>
                services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddPostgRest(services, new PostgRestOptions
                    {
                        Connection = TestingConnection,
                        MatchParamsByQueryStringKeyWhen = info =>  info.RouteName == "api/values-from-params"
                    });
        }

        public class LifeCycle : ILifeCycle
        {
            public void BuildUp() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"

            create function rest__get_values_from_params(_int int, _text text, _timestamp timestamp, _unbound text) returns json as $$
            begin
                return json_build_object(
                    'first', _int,
                    'second', _text,
                    'third', _timestamp,
                    'fourth', _unbound
                );
            end $$ language plpgsql;

            ");

            public void TearDown() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"

            drop function rest__get_values_from_params(int, text, timestamp, text);

            ");
        }

        public ParamsByQueryStringKeyTests(
            ITestOutputHelper output,
            AspNetCoreFixture<Services, LifeCycle> fixture) : base(output, fixture) {}

        [Fact]
        public async Task VerifyMatchByQueryStringNameResults()
        {
            var (result, _
                , _) = await RestClient.GetAsync<string>("https://localhost:5001/api/values-from-params?_int=999&_text=foobar&_timestamp=2019-05-19");
            Assert.Equal(@"{""first"" : 999, ""second"" : ""foobar"", ""third"" : ""2019-05-19T00:00:00"", ""fourth"" : null}", result);
        }
    }
}
