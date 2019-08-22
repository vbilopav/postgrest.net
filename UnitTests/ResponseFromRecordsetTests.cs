using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;
using static UnitTests.Config;

namespace UnitTests
{
    public class ResponseFromRecordSetTests : PostgRestClassFixture<ResponseFromRecordSetTests.LifeCycle>
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

            create function rest__post_setof() returns setof test_table as $$
            begin
                return query select * from test_table;
            end $$ language plpgsql;

            create function rest__post_table() returns table (id int, name text) as $$
            begin
                return query select * from test_table;
            end $$ language plpgsql;

            create function rest__put_setof() returns setof test_table as $$
            begin
                return query select * from test_table;
            end $$ language plpgsql;

            create function rest__put_table() returns table (id int, name text) as $$
            begin
                return query select * from test_table;
            end $$ language plpgsql;

            create function rest__delete_setof() returns setof test_table as $$
            begin
                return query select * from test_table;
            end $$ language plpgsql;

            create function rest__delete_table() returns table (id int, name text) as $$
            begin
                return query select * from test_table;
            end $$ language plpgsql;

            ");

            public void TearDown() => DatabaseFixture.ExecuteCommand(ConnectionType.Testing, @"
            
            drop function rest__get_setof();
            drop function rest__get_table();
            drop function rest__post_setof();
            drop function rest__post_table();
            drop function rest__put_setof();
            drop function rest__put_table();
            drop function rest__delete_setof();
            drop function rest__delete_table();
            drop table test_table;

            ");

        }

        public ResponseFromRecordSetTests(
            ITestOutputHelper output,
            AspNetCoreFixture<LifeCycle> fixture) : base(output, fixture) {}


        [Fact]
        public async Task VerifySetOfResultsForGet()
        {
            var (response, _, _) = await RestClient.GetAsync<JArray>("https://localhost:5001/api/setof");
            var list = response.Children().ToArray();
            Assert.Equal(1, list[0]["id"]);
            Assert.Equal("a", list[0]["name"]);
            Assert.Equal(2, list[1]["id"]);
            Assert.Equal("b", list[1]["name"]);
            Assert.Equal(3, list[2]["id"]);
            Assert.Equal("c", list[2]["name"]);
        }

        [Fact]
        public async Task VerifyTableResultsForGet()
        {
            var (response, _, _) = await RestClient.GetAsync<JArray>("https://localhost:5001/api/table");
            var list = response.Children().ToArray();
            Assert.Equal(1, list[0]["id"]);
            Assert.Equal("a", list[0]["name"]);
            Assert.Equal(2, list[1]["id"]);
            Assert.Equal("b", list[1]["name"]);
            Assert.Equal(3, list[2]["id"]);
            Assert.Equal("c", list[2]["name"]);
        }

        [Fact]
        public async Task VerifySetOfResultsForPost()
        {
            var (response, _, _) = await RestClient.PostAsync<JArray>("https://localhost:5001/api/setof", null);
            var list = response.Children().ToArray();
            Assert.Equal(1, list[0]["id"]);
            Assert.Equal("a", list[0]["name"]);
            Assert.Equal(2, list[1]["id"]);
            Assert.Equal("b", list[1]["name"]);
            Assert.Equal(3, list[2]["id"]);
            Assert.Equal("c", list[2]["name"]);
        }

        [Fact]
        public async Task VerifyTableResultsForPost()
        {
            var (response, _, _) = await RestClient.PutAsync<JArray>("https://localhost:5001/api/table", null);
            var list = response.Children().ToArray();
            Assert.Equal(1, list[0]["id"]);
            Assert.Equal("a", list[0]["name"]);
            Assert.Equal(2, list[1]["id"]);
            Assert.Equal("b", list[1]["name"]);
            Assert.Equal(3, list[2]["id"]);
            Assert.Equal("c", list[2]["name"]);
        }

        [Fact]
        public async Task VerifySetOfResultsForPut()
        {
            var (response, _, _) = await RestClient.PutAsync<JArray>("https://localhost:5001/api/setof", null);
            var list = response.Children().ToArray();
            Assert.Equal(1, list[0]["id"]);
            Assert.Equal("a", list[0]["name"]);
            Assert.Equal(2, list[1]["id"]);
            Assert.Equal("b", list[1]["name"]);
            Assert.Equal(3, list[2]["id"]);
            Assert.Equal("c", list[2]["name"]);
        }

        [Fact]
        public async Task VerifySetOfResultsForDelete()
        {
            var (response, _, _) = await RestClient.DeleteAsync<JArray>("https://localhost:5001/api/setof");
            var list = response.Children().ToArray();
            Assert.Equal(1, list[0]["id"]);
            Assert.Equal("a", list[0]["name"]);
            Assert.Equal(2, list[1]["id"]);
            Assert.Equal("b", list[1]["name"]);
            Assert.Equal(3, list[2]["id"]);
            Assert.Equal("c", list[2]["name"]);
        }

        [Fact]
        public async Task VerifyTableResultsForDelete()
        {
            var (response, _, _) = await RestClient.DeleteAsync<JArray>("https://localhost:5001/api/table");
            var list = response.Children().ToArray();
            Assert.Equal(1, list[0]["id"]);
            Assert.Equal("a", list[0]["name"]);
            Assert.Equal(2, list[1]["id"]);
            Assert.Equal("b", list[1]["name"]);
            Assert.Equal(3, list[2]["id"]);
            Assert.Equal("c", list[2]["name"]);
        }


    }
}
