using System;
using System.Data;
using Npgsql;
using Xunit;


namespace PostgTest.XUnit.Net
{
    [CollectionDefinition("PostgreSqlTestDatabase")]
    public class PostgreSqlFixtureCollection : ICollectionFixture<PostgreSqlFixture> { }

    public class PostgreSqlFixture
    {
        private readonly NpgsqlConnection connection;
        private readonly IPostgreSqlTestConfig config;

        public PostgreSqlFixture()
        {
            config = Config.Value;
            connection = new NpgsqlConnection(config.GetDefaultConnectionString());
            CreateTestDatabaseAndUser();
        }

        public void Dispose()
        {
            DropTestDatabaseAndUser();
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
            connection.Dispose();
        }

        private void CreateTestDatabaseAndUser()
        {
            try
            {
                ExecuteCommand($@"
                    {config.CreateTestDatabaseCommand}
                    {config.CreateTestUserCommand}
                ");
            }
            catch (PostgresException e)
            {
                if (e.SqlState == "42P04")
                {

                    DropTestDatabaseAndUser();
                }
                else
                {
                    throw;
                }
            }
        }

        private void DropTestDatabaseAndUser()
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
