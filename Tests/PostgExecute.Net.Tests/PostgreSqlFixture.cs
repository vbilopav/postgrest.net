using System;
using Npgsql;
using Xunit;

namespace PostgExecute.Net.Tests
{
    [CollectionDefinition("post_execute_test")]
    public class DatabaseFixtureCollection : ICollectionFixture<PostgreSqlFixture> { }

    public class PostgreSqlFixture : IDisposable
    {
        private const string Default =
            "Server=localhost;Database=postgres;Port=5433;User Id=postgres;Password=postgres;";
        public PostgreSqlFixture()
        {
            ConnectionString = "Server=localhost;Database=post_execute_test;Port=5433;User Id=postgres;Password=postgres;";
            CreateTestDatabase();
        }

        public string ConnectionString { get; }

        public void Dispose()
        {
            DropTestDatabase();
        }

        private void CreateTestDatabase()
        {
            try
            {
                Execute("create database post_execute_test;");
            }
            catch (PostgresException e)
            {
                switch (e.SqlState)
                {

                    case "42P04":
                        DropTestDatabase();
                        Execute("create database post_execute_test;");
                        break;
                }
            }
        }

        private void DropTestDatabase()
        {
            Execute(@"revoke connect on database post_execute_test from public;
            select pid, pg_terminate_backend(pid) from pg_stat_activity where datname = 'post_execute_test' and pid <> pg_backend_pid();
            drop database post_execute_test;");
        }


        private void Execute(string command)
        {
            using (var conn = new NpgsqlConnection(Default))
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
