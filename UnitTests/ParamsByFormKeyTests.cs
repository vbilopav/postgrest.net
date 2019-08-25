using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using PostgRest.net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VerySimpleRestClient;
using Xunit;
using Xunit.Abstractions;
using static UnitTests.Config;

namespace UnitTests
{
    public class ParamsByFormKeyTests : PostgRestClassFixture<ParamsByFormKeyTests.Services, ParamsByFormKeyTests.LifeCycle>
    {
        public class Services : IConfigureServices
        {
            public void ConfigureServices(IServiceCollection services) =>
                services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddPostgRest(new PostgRestOptions
                    {
                        Connection = TestingConnection,
                        MatchParamsByFormKeyWhen = info =>  info.RouteName == "api/values-from-form"
                    });
        }

        public class LifeCycle : ILifeCycle
        {
            public void BuildUp() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"

            create function rest__post_values_from_form(_int int, _text text, _timestamp timestamp, _unbound text) returns json as $$
            begin
                return json_build_object(
                    'first', _int,
                    'second', _text,
                    'third', _timestamp,
                    'fourth', _unbound
                );
            end $$ language plpgsql;

            create function rest__put_values_from_form(_int int, _text text, _timestamp timestamp, _unbound text) returns json as $$
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

            drop function rest__post_values_from_form(int, text, timestamp, text);
            drop function rest__put_values_from_form(int, text, timestamp, text);

            ");
        }

        public ParamsByFormKeyTests(
            ITestOutputHelper output,
            AspNetCoreFixture<Services, LifeCycle> fixture) : base(output, fixture) {}

        [Fact]
        public async Task VerifyPostMatchByFormNameResults()
        {
            var result = await SimpleClient.PostAsync("https://localhost:5001/api/values-from-form", 
                body: new Form(new
                {
                    _int = 999,
                    _text = "foobar",
                    _timestamp = "2019-05-19"
                }));
            Assert.Equal(999, result["first"]);
            Assert.Equal("foobar", result["second"]);
            Assert.Equal("2019-05-19T00:00:00", result["third"]);
            Assert.Null((string)result["fourth"]);
        }

        [Fact]
        public async Task VerifyPutMatchByFormNameResults()
        {
            var result = await SimpleClient.PutAsync("https://localhost:5001/api/values-from-form",
                body: new Form(new
                {
                    _int = 999,
                    _text = "foobar",
                    _timestamp = "2019-05-19"
                }));
            Assert.Equal(999, result["first"]);
            Assert.Equal("foobar", result["second"]);
            Assert.Equal("2019-05-19T00:00:00", result["third"]);
            Assert.Null((string)result["fourth"]);
        }
    }
}
