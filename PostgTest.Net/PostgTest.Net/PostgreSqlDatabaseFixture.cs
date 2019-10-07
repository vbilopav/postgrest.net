using System;
using Norm.Extensions;
using Npgsql;
using PostgTest.Net.Configuration;
using PostgTest.Net.Migrations;


namespace PostgTest.Net
{
    public class PostgreSqlDatabaseFixture : IFixture
    {
        public NpgsqlConnection TestConnection { get; }
        public NpgsqlConnection DefaultConnection { get; }
        public IPostgTestConfig Configuration { get; }
        public MigrationBase Migration { get; }

        public PostgreSqlDatabaseFixture()
        {
            Configuration = Config.Value;
            DefaultConnection = new NpgsqlConnection(Configuration.GetDefaultConnectionString());
            TestConnection = new NpgsqlConnection(Configuration.GetTestConnectionString());

            if (Configuration is PostgTestConfig config)
            {
                Migration = config.MigrationScriptsFixture ?? new ConfigMigration();
                
            }
            else
            {
                Migration = new ConfigMigration();
            }
            // ReSharper disable once VirtualMemberCallInConstructor
            Initialize();
        }

        public virtual void Dispose()
        {
            DropTestDatabaseAndTestUser();
            DefaultConnection.Dispose();
        }

        protected virtual void Initialize()
        {
            TryCreateTestDatabaseAndTestUser();
            RunMigrations();
        }

        protected virtual void RunMigrations() => Migration.Run(DefaultConnection, this);

        private void TryCreateTestDatabaseAndTestUser()
        {
            try
            {
                CreateTestDatabaseAndTestUser();
            }
            catch (PostgresException e)
            {
                switch (e.SqlState)
                {
                    // duplicate_database (see https://www.postgresql.org/docs/8.2/errcodes-appendix.html)
                    case "42P04":
                        DropTestDatabaseAndTestUser();
                        CreateTestDatabaseAndTestUser();
                        break;
                    // duplicate_object  (see https://www.postgresql.org/docs/8.2/errcodes-appendix.html)
                    case "42710":
                        DefaultConnection.Execute(Configuration.DropTestUserCommand);
                        CreateTestDatabaseAndTestUser();
                        break;
                    default:
                        throw;
                }
            }
            DefaultConnection.ChangeDatabase(Configuration.TestDatabase);
        }

        private void CreateTestDatabaseAndTestUser() =>
            DefaultConnection
                .Execute(Configuration.CreateTestDatabaseCommand)
                .Execute(Configuration.CreateTestUserCommand);

        private void DropTestDatabaseAndTestUser() =>
            DefaultConnection
                .Execute(Configuration.DropTestDatabaseCommand)
                .Execute(Configuration.DropTestUserCommand);
    }
}
