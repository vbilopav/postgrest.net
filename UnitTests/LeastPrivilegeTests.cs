using Npgsql;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using static UnitTests.Config;

namespace UnitTests
{
    public class LeastPrivilegeTests : PostgRestClassFixture<LeastPrivilegeTests.LifeCycle>
    {
        public class LifeCycle : ILifeCycle
        {
            public void BuildUp() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"

            create function rest__get_values_no_grant() returns json as
            $$
            begin
                return (
                    select json_build_object('values', array[1, 2, 3])
                );
            end
            $$ language plpgsql security definer;
            revoke all on function rest__get_values_no_grant() from public;

            create table test_values (i int);
            insert into test_values values (1), (2), (3);

            create function rest__get_values_with_grant() returns json as
            $$
            begin
                return (
                    select json_build_object('values', (select array_agg(i) from test_values))
                );
            end
            $$ language plpgsql security definer;
            revoke all on function rest__get_values_with_grant() from public;

            grant execute on function rest__get_values_with_grant() to testing;

            ");

            public void TearDown() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"

            drop function rest__get_values_no_grant();
            drop function rest__get_values_with_grant();
            drop table test_values;

            ");

        }

        public LeastPrivilegeTests(
            ITestOutputHelper output,
            AspNetCoreFixture<LifeCycle> fixture) : base(output, fixture) {}

        [Fact]
        public async Task VerifyNotFoundForEndpointWithNoGrant()
        {
            var (_, status, _) = await RestClient.GetAsync<object>("https://localhost:5001/api/values-no-grant");
            Assert.Equal(HttpStatusCode.NotFound, status);
        }

        public class ResponseModel
        {
            public int[] Values { get; set; }
        }

        [Fact]
        public async Task VerifyResponseForEndpointWithGrant()
        {
            var (response, status, contentType) = await RestClient.GetAsync<ResponseModel>("https://localhost:5001/api/values-with-grant");
            Assert.Equal(HttpStatusCode.OK, status);
            Assert.Equal("application/json; charset=utf-8", contentType);
            Assert.Equal(new int[3] { 1,2,3 }, response.Values);
        }

        [Fact]
        public void VerifyAccessDeniedToTable()
        {
            AssertAccessDenied("select * from test_values;");
        }

        [Fact]
        public void VerifyAccessDeniedToDynamicSql()
        {
            AssertAccessDenied(@"
            do $$
            begin
                execute 'select * from test_values';
            end
            $$;");
        }

        private void AssertAccessDenied(string sql)
        {
            bool permissionDenied = false;
            try
            {
                DatabaseFixture.ExecuteCommand(ConnectionType.Testing, sql);
            }
            catch (PostgresException e)
            {
                // insufficient_privilege, see: https://www.postgresql.org/docs/11/errcodes-appendix.html
                if (e.SqlState == "42501")
                {
                    permissionDenied = true;
                }
                else
                {
                    throw;
                }
            }
            Assert.True(permissionDenied);
        }
    }
}