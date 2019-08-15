using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using static UnitTests.Config;

namespace UnitTests
{
    public class ResponseTypeTests : PostgRestClassFixture<DefaultConfig, ResponseTypeTests.LifeCycle>
    {
        public class LifeCycle : ILifeCycle
        {
            public void BuildUp() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"

            create function rest__get_json_null() returns json as
            $$
            begin
                return null;
            end
            $$ language plpgsql;

            create function rest__get_jsonb_null() returns jsonb as
            $$
            begin
                return null;
            end
            $$ language plpgsql;

            create function rest__get_jsonb_value() returns jsonb as
            $$
            begin
                return '{""field"": ""value""}';
            end
            $$ language plpgsql;

            create function rest__get_void() returns void as
            $$
            begin
            end
            $$ language plpgsql;

            create function rest__get_text_null() returns text as
            $$
            begin
                return null;
            end
            $$ language plpgsql;

            create function rest__get_text_value() returns text as
            $$
            begin
                return 'text';
            end
            $$ language plpgsql;

            create function rest__get_integer_null() returns integer as
            $$
            begin
                return null;
            end
            $$ language plpgsql;

            create function rest__get_integer_value() returns integer as
            $$
            begin
                return 999;
            end
            $$ language plpgsql;

            create function rest__get_timestamp_null() returns timestamp as
            $$
            begin
                return null;
            end
            $$ language plpgsql;

            create function rest__get_timestamp_value() returns timestamp as
            $$
            begin
                return '2019-05-19'::timestamp;
            end
            $$ language plpgsql;

            ");

            public void TearDown() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"

            drop function rest__get_json_null();
            drop function rest__get_jsonb_null();
            drop function rest__get_jsonb_value();
            drop function rest__get_void();
            drop function rest__get_text_null();
            drop function rest__get_text_value();
            drop function rest__get_integer_null();
            drop function rest__get_integer_value();
            drop function rest__get_timestamp_null();
            drop function rest__get_timestamp_value();

            ");

        }

        public ResponseTypeTests(
            ITestOutputHelper output,
            AspNetCoreFixture<DefaultConfig, LifeCycle> fixture) : base(output, fixture) {}


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
    }
}
