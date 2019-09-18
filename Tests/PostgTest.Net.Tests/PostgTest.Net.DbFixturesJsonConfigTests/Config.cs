using System.IO;
using Xunit;

namespace PostgTest.Net.DbFixturesJsonConfigTests
{
    public class Config : PostgTestConfig
    {
        public override int Port => 5433;
    }

    [CollectionDefinition("PostgreSqlTestDatabase")]
    public class PostgreSqlFixtureCollection : ICollectionFixture<PostgreSqlDatabaseFixture>
    {
    }
}
