using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using static UnitTests.Config;

namespace UnitTests
{
    public class ResponseFromRecordsetTests : PostgRestClassFixture<ResponseFromRecordsetTests.LifeCycle>
    {
        public class LifeCycle : ILifeCycle
        {
            public void BuildUp() => DatabaseFixture.ExecuteCommand(ConnectionType.Testing, @"

            create table test_table(id int, name text);
            insert into test_table values (1, 'a'), (2, 'b'), (3, 'c');

            create function rest__get_setof() returns setof test_table as $$
            begin
                return query select * from test_table;
            end $$ language plpgsql;

            create function rest__get_table() returns table (id int, name text) as $$
            begin
                return query select * from test_table;
            end $$ language plpgsql;

            ");

            public void TearDown() => DatabaseFixture.ExecuteCommand(ConnectionType.Testing, @"
            
            drop function rest__get_setof();
            drop function rest__get_table();
            drop table test_table;

            ");

        }

        public ResponseFromRecordsetTests(
            ITestOutputHelper output,
            AspNetCoreFixture<LifeCycle> fixture) : base(output, fixture) {}


        [Fact]
        public async Task VerifySetOfResults()
        {
            var (response, _, _) = await RestClient.GetAsync<string>("https://localhost:5001/api/setof");
            Assert.Equal(@"[{""id"":1,""name"":""a""},{""id"":2,""name"":""b""},{""id"":3,""name"":""c""}]", response);
        }

        [Fact]
        public async Task VerifyTableResults()
        {
            var (response, _, _) = await RestClient.GetAsync<string>("https://localhost:5001/api/table");
            Assert.Equal(@"[{""id"":1,""name"":""a""},{""id"":2,""name"":""b""},{""id"":3,""name"":""c""}]", response);
        }
    }
}
