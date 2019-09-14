using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Xunit;

namespace PostgExecute.Net.Tests
{
    [Collection("post_execute_test")]
    public class ReadUnitTests
    {
        private readonly PostgreSqlFixture fixture;

        public ReadUnitTests(PostgreSqlFixture fixture)
        {
            this.fixture = fixture;
        }

        private void AssertResult(IEnumerable<IDictionary<string, object>> result)
        {
            var list = result.ToList();
            Assert.Equal(3, list.Count);

            Assert.Equal(1, list[0].Values.First());
            Assert.Equal("foo1", list[0]["bar"]);
            Assert.Equal(new DateTime(1977, 5, 19), list[0]["day"]);

            Assert.Equal(2, list[1].Values.First());
            Assert.Equal("foo2", list[1]["bar"]);
            Assert.Equal(new DateTime(1978, 5, 19), list[1]["day"]);

            Assert.Equal(3, list[2].Values.First());
            Assert.Equal("foo3", list[2]["bar"]);
            Assert.Equal(new DateTime(1979, 5, 19), list[2]["day"]);
        }

        [Fact]
        public void TestConnectionNoParams()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = connection.Read(
                    @"
                          select * from (
                          values 
                            (1, 'foo1', '1977-05-19'::date),
                            (2, 'foo2', '1978-05-19'::date),
                            (3, 'foo3', '1979-05-19'::date)
                          ) t(first, bar, day)");

                AssertResult(result);
            }
        }

        [Fact]
        public void TestConnectionReadResultNoParams()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = new List<IDictionary<string, object>>();
                connection.Read(@"
                          select * from (
                          values 
                            (1, 'foo1', '1977-05-19'::date),
                            (2, 'foo2', '1978-05-19'::date),
                            (3, 'foo3', '1979-05-19'::date)
                          ) t(first, bar, day)",
                     r => result.Add(r));

                AssertResult(result);
            }
        }

        [Fact]
        public void TestConnectionReadResultAndBreakNoParams()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = new List<IDictionary<string, object>>();
                connection.Read(@"
                          select * from (
                          values 
                            (1, 'foo1', '1977-05-19'::date),
                            (2, 'foo2', '1978-05-19'::date),
                            (3, 'foo3', '1979-05-19'::date)
                          ) t(first, bar, day)",
                    r =>
                    {
                        if ((int)r["first"] == 2)
                        {
                            return false;
                        }
                        result.Add(r);
                        return true;
                    });

                var list = result.ToList();
                Assert.Single(list);

                Assert.Equal(1, list[0].Values.First());
                Assert.Equal("foo1", list[0]["bar"]);
                Assert.Equal(new DateTime(1977, 5, 19), list[0]["day"]);
            }
        }

        [Fact]
        public void TestConnectionReadResultParamsArray()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = new List<IDictionary<string, object>>();
                connection.Read(
                    @"
                            select * from(
                                values
                            (@1, @t1, @d1),
                            (@2, @t2, @d2),
                            (@3, @t3, @d3)
                                ) t(first, bar, day)",
                    r => result.Add(r),
                    1, "foo1", new DateTime(1977, 5, 19),
                    2, "foo2", new DateTime(1978, 5, 19),
                    3, "foo3", new DateTime(1979, 5, 19));

                AssertResult(result);
            }
        }

        [Fact]
        public void TestConnectionReadResultAndBreakParamsArray()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = new List<IDictionary<string, object>>();
                connection.Read(
                    @"
                            select * from(
                                values
                            (@1, @t1, @d1),
                            (@2, @t2, @d2),
                            (@3, @t3, @d3)
                                ) t(first, bar, day)",
                    r =>
                    {
                        if ((int)r["first"] == 2)
                        {
                            return false;
                        }
                        result.Add(r);
                        return true;
                    },
                    1, "foo1", new DateTime(1977, 5, 19),
                    2, "foo2", new DateTime(1978, 5, 19),
                    3, "foo3", new DateTime(1979, 5, 19));

                var list = result.ToList();
                Assert.Single(list);

                Assert.Equal(1, list[0].Values.First());
                Assert.Equal("foo1", list[0]["bar"]);
                Assert.Equal(new DateTime(1977, 5, 19), list[0]["day"]);
            }
        }

        [Fact]
        public void TestConnectionReadResultParamsCollection()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = new List<IDictionary<string, object>>();
                connection.Read(
                    @"
                            select * from(
                                values
                            (@1, @t1, @d1),
                            (@2, @t2, @d2),
                            (@3, @t3, @d3)
                                ) t(first, bar, day)",
                    r => result.Add(r),
                    p => p
                        .@P("1", 1)
                        .@P("t1", "foo1")
                        .@P("d1", new DateTime(1977, 5, 19))
                        .@P("2", 2)
                        .@P("t2", "foo2")
                        .@P("d2", new DateTime(1978, 5, 19))
                        .@P("3", 3)
                        .@P("t3", "foo3")
                        .@P("d3", new DateTime(1979, 5, 19)));

                AssertResult(result);
            }
        }

        [Fact]
        public void TestConnectionReadResultAndBreakParamsCollection()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = new List<IDictionary<string, object>>();
                connection.Read(
                    @"
                            select * from(
                                values
                            (@1, @t1, @d1),
                            (@2, @t2, @d2),
                            (@3, @t3, @d3)
                                ) t(first, bar, day)",
                    r =>
                    {
                        if ((int)r["first"] == 2)
                        {
                            return false;
                        }
                        result.Add(r);
                        return true;
                    },
                    p => p
                        .@P("1", 1)
                        .@P("t1", "foo1")
                        .@P("d1", new DateTime(1977, 5, 19))
                        .@P("2", 2)
                        .@P("t2", "foo2")
                        .@P("d2", new DateTime(1978, 5, 19))
                        .@P("3", 3)
                        .@P("t3", "foo3")
                        .@P("d3", new DateTime(1979, 5, 19)));

                var list = result.ToList();
                Assert.Single(list);

                Assert.Equal(1, list[0].Values.First());
                Assert.Equal("foo1", list[0]["bar"]);
                Assert.Equal(new DateTime(1977, 5, 19), list[0]["day"]);
            }
        }

        [Fact]
        public void TestConnectionParamsArray()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = connection.Read(
                    @"
                          select * from (
                          values 
                            (@1, @t1, @d1),
                            (@2, @t2, @d2),
                            (@3, @t3, @d3)
                          ) t(first, bar, day)",
                    1, "foo1", new DateTime(1977, 5, 19),
                    2, "foo2", new DateTime(1978, 5, 19),
                    3, "foo3", new DateTime(1979, 5, 19));

                AssertResult(result);
            }
        }

        [Fact]
        public void TestConnectionParamsCollection()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = connection.Read(
                    @"
                          select * from (
                          values 
                            (@1, @t1, @d1),
                            (@2, @t2, @d2),
                            (@3, @t3, @d3)
                          ) t(first, bar, day)", p => p
                        .@P("1", 1)
                        .@P("t1", "foo1")
                        .@P("d1", new DateTime(1977, 5, 19))
                        .@P("2", 2)
                        .@P("t2", "foo2")
                        .@P("d2", new DateTime(1978, 5, 19))
                        .@P("3", 3)
                        .@P("t3", "foo3")
                        .@P("d3", new DateTime(1979, 5, 19)));

                AssertResult(result);
            }
        }

        [Fact]
        public async Task TestConnectionNoParamsAsync()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = new List<IDictionary<string, object>>();
                await connection.ReadAsync(
                    @"
                          select * from (
                          values 
                            (1, 'foo1', '1977-05-19'::date),
                            (2, 'foo2', '1978-05-19'::date),
                            (3, 'foo3', '1979-05-19'::date)
                          ) t(first, bar, day)",
                    r => result.Add(r));

                AssertResult(result);
            }
        }

        [Fact]
        public async Task TestConnectionNoParamsBreakReadAsync()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = new List<IDictionary<string, object>>();
                await connection.ReadAsync(
                    @"
                          select * from (
                          values 
                            (1, 'foo1', '1977-05-19'::date),
                            (2, 'foo2', '1978-05-19'::date),
                            (3, 'foo3', '1979-05-19'::date)
                          ) t(first, bar, day)",
                    r =>
                    {
                        if ((int)r["first"] == 2)
                        {
                            return false;
                        }
                        result.Add(r);
                        return true;
                    });

                var list = result.ToList();
                Assert.Single(list);

                Assert.Equal(1, list[0].Values.First());
                Assert.Equal("foo1", list[0]["bar"]);
                Assert.Equal(new DateTime(1977, 5, 19), list[0]["day"]);
            }
        }

        [Fact]
        public async Task TestConnectionAsyncResultNoParamsAsync()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = new List<IDictionary<string, object>>();
                await connection.ReadAsync(
                    @"
                          select * from (
                          values 
                            (1, 'foo1', '1977-05-19'::date),
                            (2, 'foo2', '1978-05-19'::date),
                            (3, 'foo3', '1979-05-19'::date)
                          ) t(first, bar, day)",
                    async r =>
                    {
                        await Task.Delay(0);
                        result.Add(r);
                    });

                AssertResult(result);
            }
        }

        [Fact]
        public async Task TestConnectionAsyncResultBreakReadNoParamsAsync()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = new List<IDictionary<string, object>>();
                await connection.ReadAsync(
                    @"
                          select * from (
                          values 
                            (1, 'foo1', '1977-05-19'::date),
                            (2, 'foo2', '1978-05-19'::date),
                            (3, 'foo3', '1979-05-19'::date)
                          ) t(first, bar, day)",
                    async r =>
                    {
                        await Task.Delay(0);
                        if ((int)r["first"] == 2)
                        {
                            return false;
                        }
                        result.Add(r);
                        return true;
                    });

                var list = result.ToList();
                Assert.Single(list);

                Assert.Equal(1, list[0].Values.First());
                Assert.Equal("foo1", list[0]["bar"]);
                Assert.Equal(new DateTime(1977, 5, 19), list[0]["day"]);
            }
        }

        [Fact]
        public async Task TestConnectionParamsArrayAsync()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = new List<IDictionary<string, object>>();
                await connection.ReadAsync(
                    @"
                          select * from (
                          values 
                            (@1, @t1, @d1),
                            (@2, @t2, @d2),
                            (@3, @t3, @d3)
                          ) t(first, bar, day)",
                    r => result.Add(r),
                    1, "foo1", new DateTime(1977, 5, 19),
                    2, "foo2", new DateTime(1978, 5, 19),
                    3, "foo3", new DateTime(1979, 5, 19));

                AssertResult(result);
            }
        }

        [Fact]
        public async Task TestConnectionAsyncResultParamsArrayAsync()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = new List<IDictionary<string, object>>();
                await connection.ReadAsync(
                    @"
                          select * from (
                          values 
                            (@1, @t1, @d1),
                            (@2, @t2, @d2),
                            (@3, @t3, @d3)
                          ) t(first, bar, day)",
                    async r =>
                    {
                        await Task.Delay(0);
                        result.Add(r);
                    },
                    1, "foo1", new DateTime(1977, 5, 19),
                    2, "foo2", new DateTime(1978, 5, 19),
                    3, "foo3", new DateTime(1979, 5, 19));

                AssertResult(result);
            }
        }

        [Fact]
        public async Task TestConnectionParamsCollectionAsync()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = new List<IDictionary<string, object>>();
                await connection.ReadAsync(
                    @"
                          select * from (
                          values 
                            (@1, @t1, @d1),
                            (@2, @t2, @d2),
                            (@3, @t3, @d3)
                          ) t(first, bar, day)",
                    r => result.Add(r),
                    p => p
                        .@P("1", 1)
                        .@P("t1", "foo1")
                        .@P("d1", new DateTime(1977, 5, 19))
                        .@P("2", 2)
                        .@P("t2", "foo2")
                        .@P("d2", new DateTime(1978, 5, 19))
                        .@P("3", 3)
                        .@P("t3", "foo3")
                        .@P("d3", new DateTime(1979, 5, 19)));

                AssertResult(result);
            }
        }

        [Fact]
        public async Task TestConnectionAsyncResultParamsCollectionAsync()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = new List<IDictionary<string, object>>();
                await connection.ReadAsync(
                    @"
                          select * from (
                          values 
                            (@1, @t1, @d1),
                            (@2, @t2, @d2),
                            (@3, @t3, @d3)
                          ) t(first, bar, day)",
                    async r =>
                    {
                        await Task.Delay(0);
                        result.Add(r);
                    },
                    p => p
                        .@P("1", 1)
                        .@P("t1", "foo1")
                        .@P("d1", new DateTime(1977, 5, 19))
                        .@P("2", 2)
                        .@P("t2", "foo2")
                        .@P("d2", new DateTime(1978, 5, 19))
                        .@P("3", 3)
                        .@P("t3", "foo3")
                        .@P("d3", new DateTime(1979, 5, 19)));

                AssertResult(result);
            }
        }

        [Fact]
        public async Task TestConnectionAsyncParamsCollectionAsync()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = new List<IDictionary<string, object>>();
                await connection.ReadAsync(
                    @"
                          select * from (
                          values 
                            (@1, @t1, @d1),
                            (@2, @t2, @d2),
                            (@3, @t3, @d3)
                          ) t(first, bar, day)",
                    r => result.Add(r),
                    async p =>
                    {
                        await Task.Delay(0);
                        p
                            .@P("1", 1)
                            .@P("t1", "foo1")
                            .@P("d1", new DateTime(1977, 5, 19))
                            .@P("2", 2)
                            .@P("t2", "foo2")
                            .@P("d2", new DateTime(1978, 5, 19))
                            .@P("3", 3)
                            .@P("t3", "foo3")
                            .@P("d3", new DateTime(1979, 5, 19));
                    });

                AssertResult(result);
            }
        }

        [Fact]
        public async Task TestConnectionAsyncResultAsyncParamsCollectionAsync()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = new List<IDictionary<string, object>>();
                await connection.ReadAsync(
                    @"
                          select * from (
                          values 
                            (@1, @t1, @d1),
                            (@2, @t2, @d2),
                            (@3, @t3, @d3)
                          ) t(first, bar, day)",
                    async r =>
                    {
                        await Task.Delay(0);
                        result.Add(r);
                    },
                    async p =>
                    {
                        await Task.Delay(0);
                        p
                            .@P("1", 1)
                            .@P("t1", "foo1")
                            .@P("d1", new DateTime(1977, 5, 19))
                            .@P("2", 2)
                            .@P("t2", "foo2")
                            .@P("d2", new DateTime(1978, 5, 19))
                            .@P("3", 3)
                            .@P("t3", "foo3")
                            .@P("d3", new DateTime(1979, 5, 19));
                    });

                AssertResult(result);
            }
        }

    }
}
