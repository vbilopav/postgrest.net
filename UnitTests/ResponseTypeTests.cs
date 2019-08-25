using System.Net;
using System.Threading.Tasks;
using VerySimpleRestClient;
using Xunit;
using Xunit.Abstractions;
using static UnitTests.Config;

namespace UnitTests
{
    public class ResponseTypeTests : PostgRestClassFixture<ResponseTypeTests.LifeCycle>
    {
        public class LifeCycle : ILifeCycle
        {
            public void BuildUp()
            {
                string Script(string verb) => string.Format(@"

                    create function rest__{0}_json_null() returns json as
                    $$
                    begin
                        return null;
                    end
                    $$ language plpgsql;

                    create function rest__{0}_jsonb_null() returns jsonb as
                    $$
                    begin
                        return null;
                    end
                    $$ language plpgsql;

                    create function rest__{0}_jsonb_value() returns jsonb as
                    $$
                    begin
                        return '{{""field"": ""value""}}';
                    end
                    $$ language plpgsql;

                    create function rest__{0}_void() returns void as
                    $$
                    begin
                    end
                    $$ language plpgsql;

                    create function rest__{0}_text_null() returns text as
                    $$
                    begin
                        return null;
                    end
                    $$ language plpgsql;

                    create function rest__{0}_text_value() returns text as
                    $$
                    begin
                        return 'text';
                    end
                    $$ language plpgsql;

                    create function rest__{0}_integer_null() returns integer as
                    $$
                    begin
                        return null;
                    end
                    $$ language plpgsql;

                    create function rest__{0}_integer_value() returns integer as
                    $$
                    begin
                        return 999;
                    end
                    $$ language plpgsql;

                    create function rest__{0}_timestamp_null() returns timestamp as
                    $$
                    begin
                        return null;
                    end
                    $$ language plpgsql;

                    create function rest__{0}_timestamp_value() returns timestamp as
                    $$
                    begin
                        return '2019-05-19'::timestamp;
                    end
                    $$ language plpgsql;

                    ", verb);
                DatabaseFixture.ExecuteCommand(
                    ConnectionType.PostgresTesting, 
                    $"{Script("get")}\n\n{Script("post")}\n\n{Script("put")}\n\n{Script("delete")}");
            }

            public void TearDown()
            {
                string Script(string verb) => string.Format(@"
                    drop function rest__{0}_json_null();
                    drop function rest__{0}_jsonb_null();
                    drop function rest__{0}_jsonb_value();
                    drop function rest__{0}_void();
                    drop function rest__{0}_text_null();
                    drop function rest__{0}_text_value();
                    drop function rest__{0}_integer_null();
                    drop function rest__{0}_integer_value();
                    drop function rest__{0}_timestamp_null();
                    drop function rest__{0}_timestamp_value();
                ", verb);
                DatabaseFixture.ExecuteCommand(
                    ConnectionType.PostgresTesting, 
                    $"{Script("get")}\n\n{Script("post")}\n\n{Script("put")}\n\n{Script("delete")}");
            }
        }

        public ResponseTypeTests(
            ITestOutputHelper output,
            AspNetCoreFixture<LifeCycle> fixture) : base(output, fixture) {}


        // get
        [Fact]
        public async Task VerifyGetJsonNull()
        {
            var (result, response) = await Client.GetAsync<string>("https://localhost:5001/api/json-null");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8", response.ContentType);
            Assert.Equal("{}", result);
        }

        [Fact]
        public async Task VerifyGetJsonbNull()
        {
            var (result, response) = await Client.GetAsync<string>("https://localhost:5001/api/jsonb-null");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8", response.ContentType);
            Assert.Equal("{}", result);
        }

        [Fact]
        public async Task VerifyGetJsonbValue()
        {
            var (result, response) = await Client.GetAsync<string>("https://localhost:5001/api/jsonb-value");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8", response.ContentType);
            Assert.Equal(@"{""field"": ""value""}", result);
        }

        [Fact]
        public async Task VerifyGetVoid()
        {
            var (result, response) = await Client.GetAsync<string>("https://localhost:5001/api/void");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Empty(result);
        }

        [Fact]
        public async Task VerifyGetTextNull()
        {
            var (result, response) = await Client.GetAsync<string>("https://localhost:5001/api/text-null");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Empty(result);
        }

        [Fact]
        public async Task VerifyGetTextValue()
        {
            var (result, response) = await Client.GetAsync<string>("https://localhost:5001/api/text-value");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Equal("text", result);
        }

        [Fact]
        public async Task VerifyGetIntegerNull()
        {
            var (result, response) = await Client.GetAsync<string>("https://localhost:5001/api/integer-null");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Empty(result);
        }

        [Fact]
        public async Task VerifyGetIntegerValue()
        {
            var (result, response) = await Client.GetAsync<string>("https://localhost:5001/api/integer-value");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Equal("999", result);
        }

        [Fact]
        public async Task VerifyGetTimestampNull()
        {
            var(result, response) = await Client.GetAsync<string>("https://localhost:5001/api/timestamp-null");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Empty(result);
        }

        [Fact]
        public async Task VerifyGetTimestampValue()
        {
            var (result, response) =  await Client.GetAsync<string>("https://localhost:5001/api/timestamp-value");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Equal("2019-05-19T00:00:00", result);
        }

        //post
        [Fact]
        public async Task VerifyPostJsonNull()
        {
            var (result, response) = await Client.PostAsync<string>("https://localhost:5001/api/json-null");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8", response.ContentType);
            Assert.Equal("{}", result);
        }

        [Fact]
        public async Task VerifyPostJsonbNull()
        {
            var (result, response) = await Client.PostAsync<string>("https://localhost:5001/api/jsonb-null");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8", response.ContentType);
            Assert.Equal("{}", result);
        }

        [Fact]
        public async Task VerifyPostJsonbValue()
        {
            var (result, response) = await Client.PostAsync<string>("https://localhost:5001/api/jsonb-value");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8", response.ContentType);
            Assert.Equal(@"{""field"": ""value""}", result);
        }

        [Fact]
        public async Task VerifyPostVoid()
        {
            var (result, response) = await Client.PostAsync<string>("https://localhost:5001/api/void");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Empty(result);
        }

        [Fact]
        public async Task VerifyPostTextNull()
        {
            var (result, response) = await Client.PostAsync<string>("https://localhost:5001/api/text-null");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Empty(result);
        }

        [Fact]
        public async Task VerifyPostTextValue()
        {
            var (result, response) = await Client.PostAsync<string>("https://localhost:5001/api/text-value");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Equal("text", result);
        }

        [Fact]
        public async Task VerifyPostIntegerNull()
        {
            var (result, response) = await Client.PostAsync<string>("https://localhost:5001/api/integer-null");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Empty(result);
        }

        [Fact]
        public async Task VerifyPostIntegerValue()
        {
            var (result, response) = await Client.PostAsync<string>("https://localhost:5001/api/integer-value");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Equal("999", result);
        }

        [Fact]
        public async Task VerifyPostTimestampNull()
        {
            var (result, response) = await Client.PostAsync<string>("https://localhost:5001/api/timestamp-null");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Empty(result);
        }

        [Fact]
        public async Task VerifyPostTimestampValue()
        {
            var (result, response) = await Client.PostAsync<string>("https://localhost:5001/api/timestamp-value");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Equal("2019-05-19T00:00:00", result);
        }


        //put
        [Fact]
        public async Task VerifyPutJsonNull()
        {
            var (result, response) = await Client.PutAsync<string>("https://localhost:5001/api/json-null");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8", response.ContentType);
            Assert.Equal("{}", result);
        }

        [Fact]
        public async Task VerifyPutJsonbNull()
        {
            var (result, response) = await Client.PutAsync<string>("https://localhost:5001/api/jsonb-null");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8", response.ContentType);
            Assert.Equal("{}", result);
        }

        [Fact]
        public async Task VerifyPutJsonbValue()
        {
            var (result, response) = await Client.PutAsync<string>("https://localhost:5001/api/jsonb-value");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8", response.ContentType);
            Assert.Equal(@"{""field"": ""value""}", result);
        }

        [Fact]
        public async Task VerifyPutVoid()
        {
            var (result, response) = await Client.PutAsync<string>("https://localhost:5001/api/void");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Empty(result);
        }

        [Fact]
        public async Task VerifyPutTextNull()
        {
            var (result, response) = await Client.PutAsync<string>("https://localhost:5001/api/text-null");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Empty(result);
        }

        [Fact]
        public async Task VerifyPutTextValue()
        {
            var (result, response) = await Client.PutAsync<string>("https://localhost:5001/api/text-value");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Equal("text", result);
        }

        [Fact]
        public async Task VerifyPutIntegerNull()
        {
            var (result, response) = await Client.PutAsync<string>("https://localhost:5001/api/integer-null");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Empty(result);
        }

        [Fact]
        public async Task VerifyPutIntegerValue()
        {
            var (result, response) = await Client.PutAsync<string>("https://localhost:5001/api/integer-value");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Equal("999", result);
        }

        [Fact]
        public async Task VerifyPutTimestampNull()
        {
            var (result, response) = await Client.PutAsync<string>("https://localhost:5001/api/timestamp-null");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Empty(result);
        }

        [Fact]
        public async Task VerifyPutTimestampValue()
        {
            var (result, response) = await Client.PutAsync<string>("https://localhost:5001/api/timestamp-value");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Equal("2019-05-19T00:00:00", result);
        }

        // delete
        [Fact]
        public async Task VerifyDeleteJsonNull()
        {
            var (result, response) = await Client.DeleteAsync<string>("https://localhost:5001/api/json-null");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8", response.ContentType);
            Assert.Equal("{}", result);
        }

        [Fact]
        public async Task VerifyDeleteJsonbNull()
        {
            var (result, response) = await Client.DeleteAsync<string>("https://localhost:5001/api/jsonb-null");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8", response.ContentType);
            Assert.Equal("{}", result);
        }

        [Fact]
        public async Task VerifyDeleteJsonbValue()
        {
            var (result, response) = await Client.DeleteAsync<string>("https://localhost:5001/api/jsonb-value");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8", response.ContentType);
            Assert.Equal(@"{""field"": ""value""}", result);
        }

        [Fact]
        public async Task VerifyDeleteVoid()
        {
            var (result, response) = await Client.DeleteAsync<string>("https://localhost:5001/api/void");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Empty(result);
        }

        [Fact]
        public async Task VerifyDeleteTextNull()
        {
            var (result, response) = await Client.DeleteAsync<string>("https://localhost:5001/api/text-null");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Empty(result);
        }

        [Fact]
        public async Task VerifyDeleteTextValue()
        {
            var (result, response) = await Client.DeleteAsync<string>("https://localhost:5001/api/text-value");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Equal("text", result);
        }

        [Fact]
        public async Task VerifyDeleteIntegerNull()
        {
            var (result, response) = await Client.DeleteAsync<string>("https://localhost:5001/api/integer-null");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Empty(result);
        }

        [Fact]
        public async Task VerifyDeleteIntegerValue()
        {
            var (result, response) = await Client.DeleteAsync<string>("https://localhost:5001/api/integer-value");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Equal("999", result);
        }

        [Fact]
        public async Task VerifyDeleteTimestampNull()
        {
            var (result, response) = await Client.DeleteAsync<string>("https://localhost:5001/api/timestamp-null");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Empty(result);
        }

        [Fact]
        public async Task VerifyDeleteTimestampValue()
        {
            var (result, response) = await Client.DeleteAsync<string>("https://localhost:5001/api/timestamp-value");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/plain; charset=utf-8", response.ContentType);
            Assert.Equal("2019-05-19T00:00:00", result);
        }
    }
}
