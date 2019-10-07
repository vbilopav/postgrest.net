using System.IO;
using PostgTest.Net.Configuration;
using Xunit;

namespace PostgTest.Net.TransactionIsolationTests
{
    public class Config : PostgTestConfig
    {
        public override int Port => 5433;
        public override string TestDatabase => "transaction_isolation_tests";
        public override string TestUser => "transaction_isolation_tests_user";
    }

    [CollectionDefinition("PostgreSqlTestDatabase")]
    public class PostgreSqlFixtureCollection : ICollectionFixture<PostgreSqlDatabaseFixture>
    {
    }
}
