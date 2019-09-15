using System.Linq;
using Xunit;
using Xunit.Abstractions;
using PostgExecute.Net;

namespace PostgTest.Net.XUnit.Tests
{
    [Collection("PostgreSqlTestDatabase")]
    public class PostgreSqlConfigurationClassTests : IClassFixture<PostgreSqlUnitTest>
    {
        private readonly ITestOutputHelper output;
        private readonly PostgreSqlUnitTest fixture;

        public PostgreSqlConfigurationClassTests(
            ITestOutputHelper output,
            PostgreSqlUnitTest fixture)
        {
            this.fixture = fixture;
            this.output = output;
        }

        [Fact]
        public void TestDatabaseName()
        {
            var read = fixture.Connection.Read("select current_database()").ToList();
            Assert.Single(read);
            Assert.Equal(fixture.Configuration.TestDatabase, read.First()["current_database"]);
        }

        [Fact]
        public void TestSessionUserName()
        {
            var read = fixture.Connection.Read("select session_user").ToList();
            Assert.Single(read);
            Assert.Equal(fixture.Configuration.TestUser, read.First()["session_user"]);
        }

        [Fact]
        public void TestCurrentUserName()
        {
            var read = fixture.Connection.Read("select current_user").ToList();
            Assert.Single(read);
            Assert.Equal(fixture.Configuration.TestUser, read.First()["current_user"]);
        }

        [Fact]
        public void DumpBackendPid()
        {
            var read = fixture.Connection.Read("select pg_backend_pid()").ToList();
            Assert.Single(read);
            output.WriteLine($"pg_backend_pid = {read.First()["pg_backend_pid"]}");
        }
    }
}
