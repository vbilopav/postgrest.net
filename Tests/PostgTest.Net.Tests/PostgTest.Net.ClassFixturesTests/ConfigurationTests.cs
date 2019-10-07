using System.Linq;
using Npgsql;
using Norm.Extensions;
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
            Assert.Equal(fixture.Configuration.TestDatabase, fixture.TestConnection.Single<string>("select current_database()"));
        }

        [Fact]
        public void TestSessionUserName()
        {
            Assert.Equal(fixture.Configuration.TestUser, fixture.TestConnection.Single<string>("select session_user"));
        }

        [Fact]
        public void TestCurrentUserName()
        {
            Assert.Equal(fixture.Configuration.TestUser, fixture.TestConnection.Single<string>("select current_user"));
        }

        [Fact]
        public void TestCompaniesTableNotExists()
        {
            var read = fixture.DefaultConnection.Read(
                "select table_name from information_schema.tables where table_name = 'companies'");
            Assert.Empty(read);
        }

        [Fact]
        public void DumpBackendPid()
        {
            output.WriteLine($"pg_backend_pid = {fixture.TestConnection.Single<int>("select pg_backend_pid()")}");
        }
    }
}
