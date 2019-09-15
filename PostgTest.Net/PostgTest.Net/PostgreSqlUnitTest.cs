using System;
using Npgsql;
using PostgExecute.Net;

namespace PostgTest.Net
{
    public class PostgreSqlUnitTest : IDisposable
    {
        public PostgreSqlDatabaseFixture DatabaseFixture { get; private set; }
        public IPostgTestConfig Configuration => DatabaseFixture.Configuration;
        public NpgsqlConnection Connection => this.DatabaseFixture.TestConnection;

        public PostgreSqlUnitTest(PostgreSqlDatabaseFixture fixture)
        {
            if (fixture != null)
            {
                Initialize(fixture);
            }
        }

        public void Initialize(PostgreSqlDatabaseFixture databaseFixture)
        {
            this.DatabaseFixture = databaseFixture;
            this.Connection.Execute("begin");
        }

        public void Dispose()
        {
            this.Connection.Execute("rollback");
        }
    }
}
