using System.Linq;
using Xunit;
using Xunit.Abstractions;
using PostgExecute.Net;

namespace PostgTest.Net.XUnit.Tests
{
    [Collection("PostgreSqlTestDatabase")]
    public class PostgreSqlConfigurationUnitTests : PostgreSqlUnitTest
    {
        private readonly ITestOutputHelper output;

        public PostgreSqlConfigurationUnitTests(ITestOutputHelper output, PostgreSqlDatabaseFixture databaseFixture) : base(databaseFixture)
        {
            this.output = output;
        }

        [Fact]
        public void TestDatabaseName()
        {
            var read = Connection.Read("select current_database()").ToList();
            Assert.Single(read);
            Assert.Equal(Configuration.TestDatabase, read.First()["current_database"]);
        }

        [Fact]
        public void TestSessionUserName()
        {
            var read = Connection.Read("select session_user").ToList();
            Assert.Single(read);
            Assert.Equal(Configuration.TestUser, read.First()["session_user"]);
        }

        [Fact]
        public void TestCurrentUserName()
        {
            var read = Connection.Read("select current_user").ToList();
            Assert.Single(read);
            Assert.Equal(Configuration.TestUser, read.First()["current_user"]);
        }

        [Fact]
        public void DumpBackendPid()
        {
            var read = Connection.Read("select pg_backend_pid()").ToList();
            Assert.Single(read);
            output.WriteLine($"pg_backend_pid = {read.First()["pg_backend_pid"]}");
        }
    }
}
