using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PostgTest.XUnit.Net.Tests
{
    //[CollectionDefinition("PostgreSqlTestDatabase")]
    public class PostgreSqlConfigurationTests : PostgreSqlUnitTest
    {
        private readonly ITestOutputHelper output;

        public PostgreSqlConfigurationTests(ITestOutputHelper output)
        {
            this.output = output;
        }


        [Fact]
        public async Task TestDatabaseName()
        {
            int passCount = 0;
            await ReadAsync("select current_database()", reader =>
            {
                Assert.Equal(Config.TestDatabase, reader.GetString(0));
                passCount++;
            });
            Assert.Equal(1, passCount);
        }

        /*
        [Fact]
        public async Task TestDatabaseNameAndUsername()
        {
            int passCount = 0;
            await ReadAsync("select current_database(), session_user, current_user, pg_backend_pid(); ", reader =>
            {
                passCount++;
                //Assert.
            });
        }
        */
    }
}
