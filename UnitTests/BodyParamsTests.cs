using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;
using static UnitTests.Config;

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
            using (var request = new StringContent(@"{
                   ""key1"": ""value1"", 
                   ""key2"": ""value2"",
                   ""foo"": ""bar"",
                   ""1"":""2""
                }", Encoding.UTF8, "application/json"))
            {
                var (result, _, _) = await RestClient.PostAsync<JObject>("https://localhost:5001/api/return-body", request);
                Assert.Equal("value1", result["key1"]);
                Assert.Equal("value2", result["key2"]);
                Assert.Equal("bar", result["foo"]);
                Assert.Equal("2", result["1"]);
            }
        }

        [Fact]
        public async Task VerifyPostJsonBodyAdditional()
        {
            using (var request = new StringContent(@"{
                   ""key1"": ""value1"", 
                   ""key2"": ""value2"",
                   ""foo"": ""bar"",
                   ""1"":""2""
                }", Encoding.UTF8, "application/json"))
            {
                var (result, _, _) = await RestClient.PostAsync<JObject>("https://localhost:5001/api/return-body-additional", request);
                Assert.Equal("value1", result["key1"]);
                Assert.Equal("value2", result["key2"]);
                Assert.Equal("bar", result["foo"]);
                Assert.Equal("2", result["1"]);
            }
        }

        [Fact]
        public async Task VerifyPutJsonBody()
        {
            using (var request = new StringContent(@"{
                   ""key1"": ""value1"", 
                   ""key2"": ""value2"",
                   ""foo"": ""bar"",
                   ""1"":""2""
                }", Encoding.UTF8, "application/json"))
            {
                var (result, _, _) = await RestClient.PutAsync<JObject>("https://localhost:5001/api/return-body", request);
                Assert.Equal("value1", result["key1"]);
                Assert.Equal("value2", result["key2"]);
                Assert.Equal("bar", result["foo"]);
                Assert.Equal("2", result["1"]);
            }
        }

        [Fact]
        public async Task VerifyPutJsonBodyAdditional()
        {
            using (var request = new StringContent(@"{
                   ""key1"": ""value1"", 
                   ""key2"": ""value2"",
                   ""foo"": ""bar"",
                   ""1"":""2""
                }", Encoding.UTF8, "application/json"))
            {
                var (result, _, _) = await RestClient.PutAsync<JObject>("https://localhost:5001/api/return-body-additional", request);
                Assert.Equal("value1", result["key1"]);
                Assert.Equal("value2", result["key2"]);
                Assert.Equal("bar", result["foo"]);
                Assert.Equal("2", result["1"]);
            }
        }

        [Fact]
        public async Task VerifyPostFormBody()
        {
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent("value1"), "key1");
                formData.Add(new StringContent("value2"), "key2");
                formData.Add(new StringContent("bar"), "foo");
                formData.Add(new StringContent("2"), "1");

                var (result, _, _) = await RestClient.PostAsync<JObject>("https://localhost:5001/api/return-body", formData);

                Assert.Equal("value1", result["key1"]);
                Assert.Equal("value2", result["key2"]);
                Assert.Equal("bar", result["foo"]);
                Assert.Equal("2", result["1"]);
            }
        }

        [Fact]
        public async Task VerifyPostFormBodyAdditional()
        {
            using (var request = new StringContent(@"{
                   ""key1"": ""value1"", 
                   ""key2"": ""value2"",
                   ""foo"": ""bar"",
                   ""1"":""2""
                }", Encoding.UTF8, "application/json"))
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent("value1"), "key1");
                formData.Add(new StringContent("value2"), "key2");
                formData.Add(new StringContent("bar"), "foo");
                formData.Add(new StringContent("2"), "1");
                
                var (result, _, _) = await RestClient.PostAsync<JObject>("https://localhost:5001/api/return-body-additional", request);
                Assert.Equal("value1", result["key1"]);
                Assert.Equal("value2", result["key2"]);
                Assert.Equal("bar", result["foo"]);
                Assert.Equal("2", result["1"]);
            }
        }

        [Fact]
        public async Task VerifyPutFormBody()
        {
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent("value1"), "key1");
                formData.Add(new StringContent("value2"), "key2");
                formData.Add(new StringContent("bar"), "foo");
                formData.Add(new StringContent("2"), "1");

                var (result, _, _) = await RestClient.PutAsync<JObject>("https://localhost:5001/api/return-body", formData);

                Assert.Equal("value1", result["key1"]);
                Assert.Equal("value2", result["key2"]);
                Assert.Equal("bar", result["foo"]);
                Assert.Equal("2", result["1"]);
            }
        }

        [Fact]
        public async Task VerifyPutFormBodyAdditional()
        {
            using (var request = new StringContent(@"{
                   ""key1"": ""value1"", 
                   ""key2"": ""value2"",
                   ""foo"": ""bar"",
                   ""1"":""2""
                }", Encoding.UTF8, "application/json"))
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent("value1"), "key1");
                formData.Add(new StringContent("value2"), "key2");
                formData.Add(new StringContent("bar"), "foo");
                formData.Add(new StringContent("2"), "1");

                var (result, _, _) = await RestClient.PutAsync<JObject>("https://localhost:5001/api/return-body-additional", request);
                Assert.Equal("value1", result["key1"]);
                Assert.Equal("value2", result["key2"]);
                Assert.Equal("bar", result["foo"]);
                Assert.Equal("2", result["1"]);
            }
        }

        [Fact]
        public async Task VerifyPostTextPlainBody()
        {
            var text = "The quick brown fox jumps over the lazy dog. \n The quick brown fox jumps over the lazy dog. ";
            using (var request = new StringContent(text, Encoding.UTF8, "text/plain"))
            { 
                var (result, _, _) = await RestClient.PostAsync<string>("https://localhost:5001/api/return-plain-text", request);
                Assert.Equal(text, result);
            }
        }

        [Fact]
        public async Task VerifyPutTextPlainBody()
        {
            var text = "The quick brown fox jumps over the lazy dog. \n The quick brown fox jumps over the lazy dog. ";
            using (var request = new StringContent(text, Encoding.UTF8, "text/plain"))
            {
                var (result, _, _) = await RestClient.PutAsync<string>("https://localhost:5001/api/return-plain-text", request);
                Assert.Equal(text, result);
            }
        }
    }
}
