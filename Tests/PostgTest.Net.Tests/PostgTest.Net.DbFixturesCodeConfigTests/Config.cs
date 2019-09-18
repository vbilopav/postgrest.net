using Xunit;

namespace PostgTest.Net.DbFixturesCodeConfigTests
{
    public class Config : PostgTestConfig
    {
        public override int Port => 5433;

        public override string MigrationScriptsDir => "../../../../Scripts/MigrationFiles";

        public override string[] MigrationScriptFiles => new[] { "../../../../Scripts/insert_test_data.sql" };
    }

    [CollectionDefinition("PostgreSqlTestDatabase")]
    public class PostgreSqlFixtureCollection : ICollectionFixture<PostgreSqlDatabaseFixture>
    {
    }
}
