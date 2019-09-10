using Xunit;

namespace PostgTest.Net.XUnit.Tests
{
    public class Config : PostgreSqlTestConfig
    {
        public override int Port => 5433;
    }

    [CollectionDefinition("PostgreSqlTestDatabase")]
    public class PostgreSqlFixtureCollection : ICollectionFixture<PostgreSqlFixture> { }
}
