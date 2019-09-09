using System;
using System.Data;
using Npgsql;
using Xunit;


namespace PostgTest.XUnit.Net
{
    public class PostgreSqlFixture
    {
        private readonly NpgsqlConnection connection;
        private readonly IPostgreSqlTestConfig config;

        public PostgreSqlFixture()
        {
            config = Config.Value;
            connection = new NpgsqlConnection(config.GetDefaultConnectionString());
            CreateTestDatabaseAndTestUser();
        }

        public void Dispose()
        {
            ExecuteDropTestDatabaseAndTestUser();
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
            connection.Dispose();
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
            using (var cmd = new NpgsqlCommand(command, connection))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                cmd.ExecuteNonQuery();
            }
        }
    }
}
