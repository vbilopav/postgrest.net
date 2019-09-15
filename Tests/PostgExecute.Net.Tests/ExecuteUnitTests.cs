using System;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Xunit;

namespace PostgExecute.Net.Tests
{
    [Collection("post_execute_test")]
    public class ExecuteUnitTests
    {
        private readonly PostgreSqlFixture fixture;

        public ExecuteUnitTests(PostgreSqlFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void TestConnectionNoParams()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = connection
                    .Execute("begin")
                    .Execute("create table test (t text)")
                    .Execute("insert into test values ('foo')")
                    .Single("select * from test");

                Assert.Equal("foo", result.Values.First());

                connection.Execute("rollback");
                var tableMissing = false;
                try
                {
                    connection.Single("select * from test");
                }
                catch (PostgresException)
                {
                    tableMissing = true;
                }
                Assert.True(tableMissing);
            }
        }

        [Fact]
        public void TestConnectionParamsArray()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = connection
                    .Execute("begin")
                    .Execute("create table test (i int, t text, d date)")
                    .Execute("insert into test values (@i, @t, @d)", 
                        1, "foo", new DateTime(1977, 5, 19))
                    .Single("select * from test");

                Assert.Equal(1, result["i"]);
                Assert.Equal("foo", result["t"]);
                Assert.Equal(new DateTime(1977, 5, 19), result["d"]);

                connection.Execute("rollback");
                var tableMissing = false;
                try
                {
                    connection.Single("select * from test");
                }
                catch (PostgresException)
                {
                    tableMissing = true;
                }
                Assert.True(tableMissing);
            }
        }

        [Fact]
        public void TestConnectionParamsCollection()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = connection
                    .Execute("begin")
                    .Execute("create table test (i int, t text, d date)")
                    .Execute("insert into test values (@i, @t, @d)", p => 
                        p._("d", new DateTime(1977, 5, 19))._("t", "foo")._("i", 1))
                    .Single("select * from test");

                Assert.Equal(1, result["i"]);
                Assert.Equal("foo", result["t"]);
                Assert.Equal(new DateTime(1977, 5, 19), result["d"]);

                connection.Execute("rollback");
                var tableMissing = false;
                try
                {
                    connection.Single("select * from test");
                }
                catch (PostgresException)
                {
                    tableMissing = true;
                }
                Assert.True(tableMissing);
            }
        }

        [Fact]
        public async Task TestConnectionNoParamsAsync()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                await connection.ExecuteAsync("begin");
                await connection.ExecuteAsync("create table test (t text)");
                await connection.ExecuteAsync("insert into test values ('foo')");

                var result = await connection.SingleAsync("select * from test");

                Assert.Equal("foo", result.Values.First());

                connection.Execute("rollback");
                var tableMissing = false;
                try
                {
                    await connection.SingleAsync("select * from test");
                }
                catch (PostgresException)
                {
                    tableMissing = true;
                }
                Assert.True(tableMissing);
            }
        }

        [Fact]
        public async Task TestConnectionParamsArrayAsync()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                await connection.ExecuteAsync("begin");
                await connection.ExecuteAsync("create table test (i int, t text, d date)");
                await connection.ExecuteAsync("insert into test values (@i, @t, @d)",
                        1, "foo", new DateTime(1977, 5, 19));
                var result = await connection.SingleAsync("select * from test");

                Assert.Equal(1, result["i"]);
                Assert.Equal("foo", result["t"]);
                Assert.Equal(new DateTime(1977, 5, 19), result["d"]);

                await connection.ExecuteAsync("rollback");
                var tableMissing = false;
                try
                {
                    await connection.SingleAsync("select * from test");
                }
                catch (PostgresException)
                {
                    tableMissing = true;
                }
                Assert.True(tableMissing);
            }
        }

        [Fact]
        public async Task TestConnectionParamsCollectionAsync()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                await connection.ExecuteAsync("begin");
                await connection.ExecuteAsync("create table test (i int, t text, d date)");
                await connection.ExecuteAsync("insert into test values (@i, @t, @d)", p =>
                    p._("d", new DateTime(1977, 5, 19))._("t", "foo")._("i", 1));
                var result = await connection.SingleAsync("select * from test");

                Assert.Equal(1, result["i"]);
                Assert.Equal("foo", result["t"]);
                Assert.Equal(new DateTime(1977, 5, 19), result["d"]);

                await connection.ExecuteAsync("rollback");
                var tableMissing = false;
                try
                {
                    await connection.SingleAsync("select * from test");
                }
                catch (PostgresException)
                {
                    tableMissing = true;
                }
                Assert.True(tableMissing);
            }
        }
    }
}
