using System;
using Npgsql;
using PostgExecute.Net;

namespace PostgTest.Net
{
    public class PostgreSqlTestFixture : IDisposable
    {
        public PostgreSqlDatabaseFixture DatabaseFixture { get; }
        public IPostgTestConfig Configuration => DatabaseFixture.Configuration;
        public NpgsqlConnection Connection => this.DatabaseFixture.TestConnection;

        public PostgreSqlTestFixture(PostgreSqlDatabaseFixture fixture)
        {
            this.DatabaseFixture = fixture;
            if (!Configuration.DisableFixtureTransaction)
            {
                this.Connection.Execute("begin");
            }
        }

        public void Dispose()
        {
            if (!Configuration.DisableFixtureTransaction)
            {
                this.Connection.Execute("rollback");
            }
        }
    }
}
