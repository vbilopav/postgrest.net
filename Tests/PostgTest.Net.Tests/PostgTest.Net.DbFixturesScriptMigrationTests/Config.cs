using System.IO;
using Xunit;

namespace PostgTest.Net.DbFixturesScriptMigrationTests
{
    public class Config : PostgTestConfig
    {
        public override int Port => 5433;
        public override ScriptsFixture MigrationScriptsFixture => new TestScriptsFixture();
    }

    public class TestScriptsFixture : ScriptsFixture
    {
        public override string ScriptsDir => "../../../../Scripts/MigrationFiles";
        public override string[] ScriptFiles => new[] { "../../../../Scripts/insert_test_data.sql" };
    }

    [CollectionDefinition("PostgreSqlTestDatabase")]
    public class PostgreSqlFixtureCollection : ICollectionFixture<PostgreSqlDatabaseFixture>
    {
    }
}
