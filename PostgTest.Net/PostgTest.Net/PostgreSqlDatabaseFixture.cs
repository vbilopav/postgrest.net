using System;
using Npgsql;
using PostgExecute.Net;

namespace PostgTest.Net
{
    public class PostgreSqlDatabaseFixture
    {
        public NpgsqlConnection TestConnection { get; }
        public NpgsqlConnection DefaultConnection { get; }
        public IPostgTestConfig Configuration { get; }

        public PostgreSqlDatabaseFixture()
        {
            Configuration = Config.Value;
            DefaultConnection = new NpgsqlConnection(Configuration.GetDefaultConnectionString());
            TestConnection = new NpgsqlConnection(Configuration.GetTestConnectionString());

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
        }

        public void Dispose()
        {
            DropTestDatabaseAndTestUser();
            DefaultConnection.Dispose();
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
