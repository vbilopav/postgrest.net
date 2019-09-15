using System;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Xunit;

namespace PostgExecute.Net.Tests
{
    [Collection("post_execute_test")]
    public class SingleUnitTests
    {
        private readonly PostgreSqlFixture fixture;

        public SingleUnitTests(PostgreSqlFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void TestConnectionNoParams()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = connection.Single(
                    "select 1, 'foo' as bar, '1977-05-19'::date as day, null as null"
                    );

                Assert.Equal(1, result.Values.First());
                Assert.Equal("foo", result["bar"]);
                Assert.Equal(new DateTime(1977, 5, 19), result["day"]);
                Assert.Equal(DBNull.Value, result["null"]);
            }
        }

        [Fact]
        public void TestConnectionParamsArray()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = connection.Single(
                    @"
                    select *
                    from (
                        select 1 as first, 'foo' as bar, '1977-05-19'::date as day, null as null
                    ) as sub
                    where first = @1 and bar = @2 and day = @3
                    ", 
                    
                    1, "foo", new DateTime(1977, 5, 19));

                Assert.Equal(1, result.Values.First());
                Assert.Equal("foo", result["bar"]);
                Assert.Equal(new DateTime(1977, 5, 19), result["day"]);
                Assert.Equal(DBNull.Value, result["null"]);
            }
        }

        [Fact]
        public void TestConnectionParamsCollection()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = connection.Single(
                    @"
                    select *
                    from (
                        select 1 as first, 'foo' as bar, '1977-05-19'::date as day, null as null
                    ) as sub
                    where first = @1 and bar = @2 and day = @3
                    ", p => p
                        .Add("3", new DateTime(1977, 5, 19))
                        .Add("2", "foo")
                        .Add("1", 1));

                Assert.Equal(1, result.Values.First());
                Assert.Equal("foo", result["bar"]);
                Assert.Equal(new DateTime(1977, 5, 19), result["day"]);
                Assert.Equal(DBNull.Value, result["null"]);
            }
        }

        [Fact]
        public async Task TestConnectionNoParamsAsync()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = await connection.SingleAsync(
                    "select 1, 'foo' as bar, '1977-05-19'::date as day, null as null"
                );

                Assert.Equal(1, result.Values.First());
                Assert.Equal("foo", result["bar"]);
                Assert.Equal(new DateTime(1977, 5, 19), result["day"]);
                Assert.Equal(DBNull.Value, result["null"]);
            }
        }

        [Fact]
        public async Task TestConnectionParamsArrayAsync()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = await connection.SingleAsync(
                    @"
                    select *
                    from (
                        select 1 as first, 'foo' as bar, '1977-05-19'::date as day, null as null
                    ) as sub
                    where first = @1 and bar = @2 and day = @3
                    ",

                    1, "foo", new DateTime(1977, 5, 19));

                Assert.Equal(1, result.Values.First());
                Assert.Equal("foo", result["bar"]);
                Assert.Equal(new DateTime(1977, 5, 19), result["day"]);
                Assert.Equal(DBNull.Value, result["null"]);
            }
        }

        [Fact]
        public async Task TestConnectionParamsCollectionAsync()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = await connection.SingleAsync(
                    @"
                    select *
                    from (
                        select 1 as first, 'foo' as bar, '1977-05-19'::date as day, null as null
                    ) as sub
                    where first = @1 and bar = @2 and day = @3
                    ",
                    p => p
                            ._("1", 1)
                            ._("2", "foo")
                            ._("3", new DateTime(1977, 5, 19)));

                Assert.Equal(1, result.Values.First());
                Assert.Equal("foo", result["bar"]);
                Assert.Equal(new DateTime(1977, 5, 19), result["day"]);
                Assert.Equal(DBNull.Value, result["null"]);
            }
        }

        [Fact]
        public async Task TestConnectionAsyncParamsCollectionAsync()
        {
            using (var connection = new NpgsqlConnection(fixture.ConnectionString))
            {
                var result = await connection.SingleAsync(
                    @"
                    select *
                    from (
                        select 1 as first, 'foo' as bar, '1977-05-19'::date as day, null as null
                    ) as sub
                    where first = @1 and bar = @2 and day = @3
                    ",
                    async p =>
                    {
                        await Task.Delay(0);
                        p._("3", new DateTime(1977, 5, 19))._("2", "foo")._("1", 1);
                    });

                Assert.Equal(1, result.Values.First());
                Assert.Equal("foo", result["bar"]);
                Assert.Equal(new DateTime(1977, 5, 19), result["day"]);
                Assert.Equal(DBNull.Value, result["null"]);
            }
        }

        [Fact]
        public void TestStaticNoParams()
        {
            var result = Postg.Single(fixture.ConnectionString, 
                "select 1, 'foo' as bar, '1977-05-19'::date as day, null as null");

            Assert.Equal(1, result.Values.First());
            Assert.Equal("foo", result["bar"]);
            Assert.Equal(new DateTime(1977, 5, 19), result["day"]);
            Assert.Equal(DBNull.Value, result["null"]);
        }

        [Fact]
        public void TestStaticParamsArray()
        {
            var result = Postg.Single(fixture.ConnectionString,
                @"
                select *
                from (
                    select 1 as first, 'foo' as bar, '1977-05-19'::date as day, null as null
                ) as sub
                where first = @1 and bar = @2 and day = @3
                ",

                1, "foo", new DateTime(1977, 5, 19));

            Assert.Equal(1, result.Values.First());
            Assert.Equal("foo", result["bar"]);
            Assert.Equal(new DateTime(1977, 5, 19), result["day"]);
            Assert.Equal(DBNull.Value, result["null"]);
        }

        [Fact]
        public void TestStaticParamsCollection()
        {
            var result = Postg.Single(fixture.ConnectionString,
                @"
                select *
                from (
                    select 1 as first, 'foo' as bar, '1977-05-19'::date as day, null as null
                ) as sub
                where first = @1 and bar = @2 and day = @3
                ",
                p => p
                    ._("3", new DateTime(1977, 5, 19))
                    ._("2", "foo")
                    ._("1", 1));

            Assert.Equal(1, result.Values.First());
            Assert.Equal("foo", result["bar"]);
            Assert.Equal(new DateTime(1977, 5, 19), result["day"]);
            Assert.Equal(DBNull.Value, result["null"]);
        }

        [Fact]
        public async Task TestStaticNoParamsAsync()
        {
            var result = await Postg.SingleAsync(fixture.ConnectionString,
                "select 1, 'foo' as bar, '1977-05-19'::date as day, null as null"
            );

            Assert.Equal(1, result.Values.First());
            Assert.Equal("foo", result["bar"]);
            Assert.Equal(new DateTime(1977, 5, 19), result["day"]);
            Assert.Equal(DBNull.Value, result["null"]);
        }

        [Fact]
        public async Task TestStaticParamsArrayAsync()
        {
            var result = await Postg.SingleAsync(fixture.ConnectionString,
                @"
                select *
                from (
                    select 1 as first, 'foo' as bar, '1977-05-19'::date as day, null as null
                ) as sub
                where first = @1 and bar = @2 and day = @3
                ",

                1, "foo", new DateTime(1977, 5, 19));

            Assert.Equal(1, result.Values.First());
            Assert.Equal("foo", result["bar"]);
            Assert.Equal(new DateTime(1977, 5, 19), result["day"]);
            Assert.Equal(DBNull.Value, result["null"]);
        }

        [Fact]
        public async Task TestStaticParamsCollectionAsync()
        {

            var result = await Postg.SingleAsync(fixture.ConnectionString,
                @"
                select *
                from (
                    select 1 as first, 'foo' as bar, '1977-05-19'::date as day, null as null
                ) as sub
                where first = @1 and bar = @2 and day = @3
                ",
                p => p
                    ._("3", new DateTime(1977, 5, 19))
                    ._("2", "foo")
                    ._("1", 1));

            Assert.Equal(1, result.Values.First());
            Assert.Equal("foo", result["bar"]);
            Assert.Equal(new DateTime(1977, 5, 19), result["day"]);
            Assert.Equal(DBNull.Value, result["null"]);

        }

        [Fact]
        public async Task TestStaticAsyncParamsCollectionAsync()
        {
            var result = await Postg.SingleAsync(fixture.ConnectionString,
                @"
                select *
                from (
                    select 1 as first, 'foo' as bar, '1977-05-19'::date as day, null as null
                ) as sub
                where first = @1 and bar = @2 and day = @3
                ",
                async p =>
                {
                    await Task.Delay(0);
                    p._("3", new DateTime(1977, 5, 19))._("2", "foo")._("1", 1);
                });

            Assert.Equal(1, result.Values.First());
            Assert.Equal("foo", result["bar"]);
            Assert.Equal(new DateTime(1977, 5, 19), result["day"]);
            Assert.Equal(DBNull.Value, result["null"]);
        }
    }
}
