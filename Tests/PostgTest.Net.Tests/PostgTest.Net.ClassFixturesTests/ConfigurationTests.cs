using System.Linq;
using PostgExecute.Net;
using Xunit;
using Xunit.Abstractions;

namespace PostgTest.Net.ClassFixturesTests
{
    [Collection("PostgreSqlTestDatabase")]
    public class ConfigurationTests : IClassFixture<PostgreSqlTestFixture>
    {
        private readonly ITestOutputHelper output;
        private readonly PostgreSqlTestFixture fixture;

        public ConfigurationTests(
            ITestOutputHelper output,
            PostgreSqlTestFixture fixture)
        {
            this.fixture = fixture;
            this.output = output;
        }

        [Fact]
        public void TestDatabaseName()
        {
            var read = fixture.TestConnection.Read("select current_database()").ToList();
            Assert.Single(read);
            Assert.Equal(fixture.Configuration.TestDatabase, read.First()["current_database"]);
        }

        [Fact]
        public void TestSessionUserName()
        {
            var read = fixture.TestConnection.Read("select session_user").ToList();
            Assert.Single(read);
            Assert.Equal(fixture.Configuration.TestUser, read.First()["session_user"]);
        }

        [Fact]
        public void TestCurrentUserName()
        {
            var read = fixture.TestConnection.Read("select current_user").ToList();
            Assert.Single(read);
            Assert.Equal(fixture.Configuration.TestUser, read.First()["current_user"]);
        }

        [Fact]
        public void DumpBackendPid()
        {
            var read = fixture.TestConnection.Read("select pg_backend_pid()").ToList();
            Assert.Single(read);
            output.WriteLine($"pg_backend_pid = {read.First()["pg_backend_pid"]}");
        }
    }
}
