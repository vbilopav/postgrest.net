using PostgTest.Net.Configuration;
using Xunit;

namespace PostgTest.Net.DbMigrationJsonConfigTests
{
    public class Config : PostgTestConfig
    {
        public override int Port => 5433;
        public override string TestDatabase => "db_migration_json_config_tests";
        public override string TestUser => "db_migration_json_config_tests_user";
    }

    [CollectionDefinition("PostgreSqlTestDatabase")]
    public class PostgreSqlFixtureCollection : ICollectionFixture<PostgreSqlDatabaseFixture>
    {
    }
}
