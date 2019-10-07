using System.IO;
using PostgTest.Net.Configuration;
using Xunit;

namespace PostgTest.Net.ClassFixturesTests
{
    public class Config : PostgTestConfig
    {
        public override int Port => 5433;
        public override string TestDatabase => "class_fixtures_tests";
        public override string TestUser => "class_fixtures_tests_user";
    }

    [CollectionDefinition("PostgreSqlTestDatabase")]
    public class PostgreSqlFixtureCollection : ICollectionFixture<PostgreSqlDatabaseFixture>
    {
    }
}
