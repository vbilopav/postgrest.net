using System;
using Norm.Extensions;
using Npgsql;
using PostgTest.Net.Configuration;
using PostgTest.Net.Migrations;


namespace PostgTest.Net
{
    public class PostgreSqlTestFixture : PostgreSqlTestFixture<NullMigration>
    {
        public PostgreSqlTestFixture(PostgreSqlDatabaseFixture fixture) : base(fixture)
        {
        }
    }

    public class PostgreSqlTestFixture<TMigration> : IDisposable, IFixture
        where TMigration : MigrationBase, new()
    {
        private readonly TMigration migration = new TMigration();

        public PostgreSqlDatabaseFixture DatabaseFixture { get; }
        public IPostgTestConfig Configuration => DatabaseFixture.Configuration;
        public NpgsqlConnection TestConnection => this.DatabaseFixture.TestConnection;
        public NpgsqlConnection DefaultConnection => this.DatabaseFixture.DefaultConnection;
        public MigrationBase Migration => migration;

        public PostgreSqlTestFixture(PostgreSqlDatabaseFixture fixture)
        {
            this.DatabaseFixture = fixture;
            if (!Configuration.DisableFixtureTransaction)
            {
                this.TestConnection.Execute("begin");
            }
            Migration.Run(TestConnection, this);
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
