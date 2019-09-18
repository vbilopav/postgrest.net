using System;
using Npgsql;
using PostgExecute.Net;

namespace PostgTest.Net
{
    public class PostgreSqlTestFixture : PostgreSqlTestFixture<NullScriptsFixture>
    {
        public PostgreSqlTestFixture(PostgreSqlDatabaseFixture fixture) : base(fixture)
        {
        }
    }

    public class PostgreSqlTestFixture<TMigration> : IDisposable where TMigration : ScriptsFixture, new()
    {
        public PostgreSqlDatabaseFixture DatabaseFixture { get; }
        public IPostgTestConfig Configuration => DatabaseFixture.Configuration;
        public NpgsqlConnection TestConnection => this.DatabaseFixture.TestConnection;
        public NpgsqlConnection DefaultConnection => this.DatabaseFixture.DefaultConnection;

        public PostgreSqlTestFixture(PostgreSqlDatabaseFixture fixture)
        {
            this.DatabaseFixture = fixture;
            if (!Configuration.DisableFixtureTransaction)
            {
                this.TestConnection.Execute("begin");
            }
        }

        public void Dispose()
        {
            if (!Configuration.DisableFixtureTransaction)
            {
                this.TestConnection.Execute("rollback");
            }
        }
    }
}
