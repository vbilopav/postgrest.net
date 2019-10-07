using PostgTest.Net.Configuration;
using PostgTest.Net.Migrations;
using Xunit;

namespace PostgTest.Net.DbMigrationCodeClassTests
{
    public class Config : PostgTestConfig
    {
        public override int Port => 5433;
        public override string TestDatabase => "db_migration_code_class_tests";
        public override string TestUser => "db_migration_code_class_tests_user";
        public override MigrationBase MigrationScriptsFixture => new DatabaseMigration();
    }

    public class DatabaseMigration : MigrationBase
    {
        public override string ScriptsDir => "../../../../Scripts/MigrationFiles";
        public override string[] ScriptFiles => new[] { "../../../../Scripts/insert_test_data.sql" };
    }

    [CollectionDefinition("PostgreSqlTestDatabase")]
    public class PostgreSqlFixtureCollection : ICollectionFixture<PostgreSqlDatabaseFixture>
    {
    }
}
