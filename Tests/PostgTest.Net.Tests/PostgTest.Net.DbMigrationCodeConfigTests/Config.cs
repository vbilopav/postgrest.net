using PostgTest.Net.Configuration;
using Xunit;

namespace PostgTest.Net.DbMigrationCodeConfigTests
{
    public class Config : PostgTestConfig
    {
        public override int Port => 5433;
        public override string TestDatabase => "db_migration_code_config_tests";
        public override string TestUser => "db_migration_code_config_tests_user";

        public override string MigrationScriptsDir => "../../../../Scripts/MigrationFiles";

        public override string[] MigrationScriptFiles => new[] { "../../../../Scripts/insert_test_data.sql" };
    }

    [CollectionDefinition("PostgreSqlTestDatabase")]
    public class PostgreSqlFixtureCollection : ICollectionFixture<PostgreSqlDatabaseFixture>
    {
    }
}
