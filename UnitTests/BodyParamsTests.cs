using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;
using static UnitTests.Config;
using VerySimpleRestClient;

namespace UnitTests
{
    public class BodyParamsTests : PostgRestClassFixture<BodyParamsTests.LifeCycle>
    {
        public class LifeCycle : ILifeCycle
        {
            public void BuildUp() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"

            create function rest__post_return_body(_body json) returns json as $$
            begin
                return _body;
            end $$ language plpgsql;

            create function rest__post_return_body_additional(_body json, _additional text) returns json as $$
            begin
                return _body::jsonb || format('{""additional"": ""%s""}', _additional)::jsonb;
            end $$ language plpgsql;


            create function rest__put_return_body(_body json) returns json as $$
            begin
                return _body;
            end $$ language plpgsql;

            create function rest__put_return_body_additional(_body json, _additional text) returns json as $$
            begin
                return _body::jsonb || format('{""additional"": ""%s""}', _additional)::jsonb;
            end $$ language plpgsql;

            create function rest__post_return_plain_text(_body text) returns text as $$
            begin
                return _body;
            end $$ language plpgsql;

            create function rest__put_return_plain_text(_body text) returns text as $$
            begin
                return _body;
            end $$ language plpgsql;

            ");

            public void TearDown() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"

            drop function rest__post_return_body(json);
            drop function rest__post_return_body_additional(json, text);

            drop function rest__put_return_body(json);
            drop function rest__put_return_body_additional(json, text);

            drop function rest__post_return_plain_text(text);
            drop function rest__put_return_plain_text(text);

            ");
        }

        public BodyParamsTests(
            ITestOutputHelper output,
            AspNetCoreFixture<LifeCycle> fixture) : base(output, fixture) {}


        [Fact]
        public async Task VerifyPostJsonBody()
        {
            var result = await SimpleClient.PostAsync("https://localhost:5001/api/return-body",
                body: new Json(new
            {
                key1 = "value1",
                key2 = "value2",
                foo = "bar"
            }));
            Assert.Equal("value1", result["key1"]);
            Assert.Equal("value2", result["key2"]);
            Assert.Equal("bar", result["foo"]);
        }

        [Fact]
        public async Task VerifyPostJsonBodyAdditional()
        {
            var result = await SimpleClient.PostAsync("https://localhost:5001/api/return-body-additional",
                body: new Json(new
                {
                    key1 = "value1",
                    key2 = "value2",
                    foo = "bar"
                }));
            Assert.Equal("value1", result["key1"]);
            Assert.Equal("value2", result["key2"]);
            Assert.Equal("bar", result["foo"]);
            Assert.Equal("", result["additional"]);
        }

        [Fact]
        public async Task VerifyPutJsonBody()
        {
            var result = await SimpleClient.PutAsync("https://localhost:5001/api/return-body",
                body: new Json(new
                {
                    key1 = "value1",
                    key2 = "value2",
                    foo = "bar"
                }));
            Assert.Equal("value1", result["key1"]);
            Assert.Equal("value2", result["key2"]);
            Assert.Equal("bar", result["foo"]);
        }

        [Fact]
        public async Task VerifyPutJsonBodyAdditional()
        {
            var result = await SimpleClient.PutAsync("https://localhost:5001/api/return-body-additional",
                body: new Json(new
                {
                    key1 = "value1",
                    key2 = "value2",
                    foo = "bar"
                }));
            Assert.Equal("value1", result["key1"]);
            Assert.Equal("value2", result["key2"]);
            Assert.Equal("bar", result["foo"]);
            Assert.Equal("", result["additional"]);
        }

        [Fact]
        public async Task VerifyPostFormBody()
        {
            var result = await SimpleClient.PostAsync("https://localhost:5001/api/return-body",
                body: new Form(new
                {
                    key1 = "value1",
                    key2 = "value2",
                    foo = "bar"
                }));
            Assert.Equal("value1", result["key1"]);
            Assert.Equal("value2", result["key2"]);
            Assert.Equal("bar", result["foo"]);
        }

        [Fact]
        public async Task VerifyPostFormBodyAdditional()
        {
            var result = await SimpleClient.PostAsync("https://localhost:5001/api/return-body-additional",
                body: new Form(new
                {
                    key1 = "value1",
                    key2 = "value2",
                    foo = "bar"
                }));
            Assert.Equal("value1", result["key1"]);
            Assert.Equal("value2", result["key2"]);
            Assert.Equal("bar", result["foo"]);
            Assert.Equal("", result["additional"]);
        }

        [Fact]
        public async Task VerifyPutFormBody()
        {
            var result = await SimpleClient.PutAsync("https://localhost:5001/api/return-body",
                body: new Form(new
                {
                    key1 = "value1",
                    key2 = "value2",
                    foo = "bar"
                }));
            Assert.Equal("value1", result["key1"]);
            Assert.Equal("value2", result["key2"]);
            Assert.Equal("bar", result["foo"]);
        }

        [Fact]
        public async Task VerifyPutFormBodyAdditional()
        {
            var result = await SimpleClient.PutAsync("https://localhost:5001/api/return-body-additional",
                body: new Form(new
                {
                    key1 = "value1",
                    key2 = "value2",
                    foo = "bar"
                }));
            Assert.Equal("value1", result["key1"]);
            Assert.Equal("value2", result["key2"]);
            Assert.Equal("bar", result["foo"]);
            Assert.Equal("", result["additional"]);
        }

        [Fact]
        public async Task VerifyPostTextPlainBody()
        {
            var text = "The quick brown fox jumps over the lazy dog. \n The quick brown fox jumps over the lazy dog. ";
            var result = await SimpleClient.PostAsync<string>("https://localhost:5001/api/return-plain-text", body: new TextPlain(text));
            Assert.Equal(text, result);
        }

        [Fact]
        public async Task VerifyPutTextPlainBody()
        {
            var text = "The quick brown fox jumps over the lazy dog. \n The quick brown fox jumps over the lazy dog. ";
            var result = await SimpleClient.PutAsync<string>("https://localhost:5001/api/return-plain-text", body: new TextPlain(text));
            Assert.Equal(text, result);
        }
    }
}
