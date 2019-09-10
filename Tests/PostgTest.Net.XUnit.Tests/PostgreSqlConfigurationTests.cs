using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace PostgTest.Net.XUnit.Tests
{
    [Collection("PostgreSqlTestDatabase")]
    public class PostgreSqlConfigurationTests : PostgreSqlUnitTest
    {
        private readonly ITestOutputHelper output;

        public PostgreSqlConfigurationTests(ITestOutputHelper output, PostgreSqlFixture fixture) : base(fixture)
        {
            this.output = output;
        }

        [Fact]
        public void TestDatabaseName()
        {
            var read = Read("select current_database()").ToList();
            Assert.Single(read);
            Assert.Equal(Config.TestDatabase, read.First()["current_database"]);
        }

        [Fact]
        public void TestSessionUserName()
        {
            var read = Read("select session_user").ToList();
            Assert.Single(read);
            Assert.Equal(Config.TestUser, read.First()["session_user"]);
        }

        [Fact]
        public void TestCurrentUserName()
        {
            var read = Read("select current_user").ToList();
            Assert.Single(read);
            Assert.Equal(Config.TestUser, read.First()["current_user"]);
        }

        [Fact]
        public void DumpBackendPid()
        {
            var read = Read("select pg_backend_pid()").ToList();
            Assert.Single(read);
            output.WriteLine($"pg_backend_pid = {read.First()["pg_backend_pid"]}");
        }
    }
}
