using System.Net;
using System.Threading.Tasks;
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
            var (response, status, contentType) = await RestClient.GetAsync<string>("https://localhost:5001/api/json-null");
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("application/json; charset=utf-8", contentType);
            Assert.Equal("{}", response);
        }

        [Fact]
        public async Task VerifyGetJsonbNull()
        {
            var (response, status, contentType) = await RestClient.GetAsync<string>("https://localhost:5001/api/jsonb-null");
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("application/json; charset=utf-8", contentType);
            Assert.Equal("{}", response);
        }

        [Fact]
        public async Task VerifyGetJsonbValue()
        {
            var (response, status, contentType) = await RestClient.GetAsync<string>("https://localhost:5001/api/jsonb-value");
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("application/json; charset=utf-8", contentType);
            Assert.Equal(@"{""field"": ""value""}", response);
        }

        [Fact]
        public async Task VerifyGetVoid()
        {
            var (response, status, contentType) = await RestClient.GetAsync<string>("https://localhost:5001/api/void");
            Assert.Equal(HttpStatusCode.NoContent, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Empty(response);
        }

        [Fact]
        public async Task VerifyGetTextNull()
        {
            var (response, status, contentType) = await RestClient.GetAsync<string>("https://localhost:5001/api/text-null");
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Empty(response);
        }

        [Fact]
        public async Task VerifyGetTextValue()
        {
            var (response, status, contentType) = await RestClient.GetAsync<string>("https://localhost:5001/api/text-value");
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Equal("text", response);
        }

        [Fact]
        public async Task VerifyGetIntegerNull()
        {
            var (response, status, contentType) = await RestClient.GetAsync<string>("https://localhost:5001/api/integer-null");
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Empty(response);
        }

        [Fact]
        public async Task VerifyGetIntegerValue()
        {
            var (response, status, contentType) = await RestClient.GetAsync<string>("https://localhost:5001/api/integer-value");
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Equal("999", response);
        }

        [Fact]
        public async Task VerifyGetTimestampNull()
        {
            var (response, status, contentType) = await RestClient.GetAsync<string>("https://localhost:5001/api/timestamp-null");
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Empty(response);
        }

        [Fact]
        public async Task VerifyGetTimestampValue()
        {
            var (response, status, contentType) = await RestClient.GetAsync<string>("https://localhost:5001/api/timestamp-value");
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Equal("2019-05-19T00:00:00", response);
        }

        //post
        [Fact]
        public async Task VerifyPostJsonNull()
        {
            var (response, status, contentType) = await RestClient.PostAsync<string>("https://localhost:5001/api/json-null", null);
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("application/json; charset=utf-8", contentType);
            Assert.Equal("{}", response);
        }

        [Fact]
        public async Task VerifyPostJsonbNull()
        {
            var (response, status, contentType) = await RestClient.PostAsync<string>("https://localhost:5001/api/jsonb-null", null);
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("application/json; charset=utf-8", contentType);
            Assert.Equal("{}", response);
        }

        [Fact]
        public async Task VerifyPostJsonbValue()
        {
            var (response, status, contentType) = await RestClient.PostAsync<string>("https://localhost:5001/api/jsonb-value", null);
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("application/json; charset=utf-8", contentType);
            Assert.Equal(@"{""field"": ""value""}", response);
        }

        [Fact]
        public async Task VerifyPostVoid()
        {
            var (response, status, contentType) = await RestClient.PostAsync<string>("https://localhost:5001/api/void", null);
            Assert.Equal(HttpStatusCode.NoContent, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Empty(response);
        }

        [Fact]
        public async Task VerifyPostTextNull()
        {
            var (response, status, contentType) = await RestClient.PostAsync<string>("https://localhost:5001/api/text-null", null);
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Empty(response);
        }

        [Fact]
        public async Task VerifyPostTextValue()
        {
            var (response, status, contentType) = await RestClient.PostAsync<string>("https://localhost:5001/api/text-value", null);
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Equal("text", response);
        }

        [Fact]
        public async Task VerifyPostIntegerNull()
        {
            var (response, status, contentType) = await RestClient.PostAsync<string>("https://localhost:5001/api/integer-null", null);
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Empty(response);
        }

        [Fact]
        public async Task VerifyPostIntegerValue()
        {
            var (response, status, contentType) = await RestClient.PostAsync<string>("https://localhost:5001/api/integer-value", null);
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Equal("999", response);
        }

        [Fact]
        public async Task VerifyPostTimestampNull()
        {
            var (response, status, contentType) = await RestClient.PostAsync<string>("https://localhost:5001/api/timestamp-null", null);
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Empty(response);
        }

        [Fact]
        public async Task VerifyPostTimestampValue()
        {
            var (response, status, contentType) = await RestClient.PostAsync<string>("https://localhost:5001/api/timestamp-value", null);
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Equal("2019-05-19T00:00:00", response);
        }


        //put
        [Fact]
        public async Task VerifyPutJsonNull()
        {
            var (response, status, contentType) = await RestClient.PutAsync<string>("https://localhost:5001/api/json-null", null);
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("application/json; charset=utf-8", contentType);
            Assert.Equal("{}", response);
        }

        [Fact]
        public async Task VerifyPutJsonbNull()
        {
            var (response, status, contentType) = await RestClient.PutAsync<string>("https://localhost:5001/api/jsonb-null", null);
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("application/json; charset=utf-8", contentType);
            Assert.Equal("{}", response);
        }

        [Fact]
        public async Task VerifyPutJsonbValue()
        {
            var (response, status, contentType) = await RestClient.PutAsync<string>("https://localhost:5001/api/jsonb-value", null);
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("application/json; charset=utf-8", contentType);
            Assert.Equal(@"{""field"": ""value""}", response);
        }

        [Fact]
        public async Task VerifyPutVoid()
        {
            var (response, status, contentType) = await RestClient.PutAsync<string>("https://localhost:5001/api/void", null);
            Assert.Equal(HttpStatusCode.NoContent, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Empty(response);
        }

        [Fact]
        public async Task VerifyPutTextNull()
        {
            var (response, status, contentType) = await RestClient.PutAsync<string>("https://localhost:5001/api/text-null", null);
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Empty(response);
        }

        [Fact]
        public async Task VerifyPutTextValue()
        {
            var (response, status, contentType) = await RestClient.PutAsync<string>("https://localhost:5001/api/text-value", null);
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Equal("text", response);
        }

        [Fact]
        public async Task VerifyPutIntegerNull()
        {
            var (response, status, contentType) = await RestClient.PutAsync<string>("https://localhost:5001/api/integer-null", null);
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Empty(response);
        }

        [Fact]
        public async Task VerifyPutIntegerValue()
        {
            var (response, status, contentType) = await RestClient.PutAsync<string>("https://localhost:5001/api/integer-value", null);
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Equal("999", response);
        }

        [Fact]
        public async Task VerifyPutTimestampNull()
        {
            var (response, status, contentType) = await RestClient.PutAsync<string>("https://localhost:5001/api/timestamp-null", null);
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Empty(response);
        }

        [Fact]
        public async Task VerifyPutTimestampValue()
        {
            var (response, status, contentType) = await RestClient.PutAsync<string>("https://localhost:5001/api/timestamp-value", null);
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Equal("2019-05-19T00:00:00", response);
        }

        // delete
        [Fact]
        public async Task VerifyDeleteJsonNull()
        {
            var (response, status, contentType) = await RestClient.DeleteAsync<string>("https://localhost:5001/api/json-null");
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("application/json; charset=utf-8", contentType);
            Assert.Equal("{}", response);
        }

        [Fact]
        public async Task VerifyDeleteJsonbNull()
        {
            var (response, status, contentType) = await RestClient.DeleteAsync<string>("https://localhost:5001/api/jsonb-null");
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("application/json; charset=utf-8", contentType);
            Assert.Equal("{}", response);
        }

        [Fact]
        public async Task VerifyDeleteJsonbValue()
        {
            var (response, status, contentType) = await RestClient.DeleteAsync<string>("https://localhost:5001/api/jsonb-value");
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("application/json; charset=utf-8", contentType);
            Assert.Equal(@"{""field"": ""value""}", response);
        }

        [Fact]
        public async Task VerifyDeleteVoid()
        {
            var (response, status, contentType) = await RestClient.DeleteAsync<string>("https://localhost:5001/api/void");
            Assert.Equal(HttpStatusCode.NoContent, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Empty(response);
        }

        [Fact]
        public async Task VerifyDeleteTextNull()
        {
            var (response, status, contentType) = await RestClient.DeleteAsync<string>("https://localhost:5001/api/text-null");
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Empty(response);
        }

        [Fact]
        public async Task VerifyDeleteTextValue()
        {
            var (response, status, contentType) = await RestClient.DeleteAsync<string>("https://localhost:5001/api/text-value");
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Equal("text", response);
        }

        [Fact]
        public async Task VerifyDeleteIntegerNull()
        {
            var (response, status, contentType) = await RestClient.DeleteAsync<string>("https://localhost:5001/api/integer-null");
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Empty(response);
        }

        [Fact]
        public async Task VerifyDeleteIntegerValue()
        {
            var (response, status, contentType) = await RestClient.DeleteAsync<string>("https://localhost:5001/api/integer-value");
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Equal("999", response);
        }

        [Fact]
        public async Task VerifyDeleteTimestampNull()
        {
            var (response, status, contentType) = await RestClient.DeleteAsync<string>("https://localhost:5001/api/timestamp-null");
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Empty(response);
        }

        [Fact]
        public async Task VerifyDeleteTimestampValue()
        {
            var (response, status, contentType) = await RestClient.DeleteAsync<string>("https://localhost:5001/api/timestamp-value");
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("text/plain; charset=utf-8", contentType);
            Assert.Equal("2019-05-19T00:00:00", response);
        }
    }
}
