using System;
using System.Data;
using Npgsql;

namespace PostgTest.Net
{
    public interface IPostgreSqlFixture : IDisposable
    {
        NpgsqlConnection Connection { get; }
    }

    public class PostgreSqlFixture : IPostgreSqlFixture
    {
        private readonly IPostgreSqlTestConfig config;

        public PostgreSqlFixture()
        {
            config = Config.Value;
            Connection = new NpgsqlConnection(config.GetTestConnectionString());
            CreateTestDatabaseAndTestUser();
        }

        public NpgsqlConnection Connection { get; }

        public void Dispose()
        {
            ExecuteDropTestDatabaseAndTestUser();
            Connection.Dispose();
        }

        private void CreateTestDatabaseAndTestUser()
        {
            try
            {
                ExecuteCreateTestDatabaseAndTestUser();
            }
            catch (PostgresException e)
            {
                switch (e.SqlState)
                {
                    // duplicate_database (see https://www.postgresql.org/docs/8.2/errcodes-appendix.html)
                    case "42P04":
                        ExecuteDropTestDatabaseAndTestUser();
                        ExecuteCreateTestDatabaseAndTestUser();
                        break;
                    // duplicate_object  (see https://www.postgresql.org/docs/8.2/errcodes-appendix.html)
                    case "42710":
                        ExecuteCommand(config.DropTestUserCommand);
                        ExecuteCreateTestDatabaseAndTestUser();
                        break;
                    default:
                        throw;
                }
            }
        }

        private void ExecuteCreateTestDatabaseAndTestUser()
        {
            ExecuteCommand($@"
                {config.CreateTestDatabaseCommand}
                {config.CreateTestUserCommand}
            ");
        }

        private void ExecuteDropTestDatabaseAndTestUser()
        {
            ExecuteCommand($@"
                {config.DropTestDatabaseCommand}
                {config.DropTestUserCommand}
            ");
        }

        private void ExecuteCommand(string command)
        {
            using (var conn = new NpgsqlConnection(config.GetDefaultConnectionString()))
            {
                using (var cmd = new NpgsqlCommand(command, conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
    }
}
