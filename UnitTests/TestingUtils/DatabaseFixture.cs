using Npgsql;
using System;
using Xunit;
using static UnitTests.Config;

namespace UnitTests
{
    public class DatabaseFixture : IDisposable
    {
        private const string CreateDbAndUser = @"
            create database testing;

            create role testing with
                login
                nosuperuser
                nocreatedb
                nocreaterole
                noinherit
                noreplication
                connection limit -1
                password 'testing';
            ";

        private const string DropDbAndUser = @"
            revoke connect on database testing from public;

            select pid, pg_terminate_backend(pid)
            from pg_stat_activity
            where datname = 'testing' AND pid <> pg_backend_pid();

            drop database testing;

            drop role testing;
            ";

        public DatabaseFixture()
        {
            try
            {
                ExecuteCommand(ConnectionType.Postgres, CreateDbAndUser);
            }
            catch (PostgresException e)
            {
                if (e.SqlState == "42P04")
                {

                    ExecuteCommand(ConnectionType.Postgres, DropDbAndUser);
                    ExecuteCommand(ConnectionType.Postgres, CreateDbAndUser);
                }
                else
                {
                    throw;
                }
            }
        }

        public void Dispose()
        {
            ExecuteCommand(ConnectionType.Postgres, DropDbAndUser);
        }

        public static void ExecuteCommand(ConnectionType type, string command)
        {
            using (var connection = new NpgsqlConnection(Connection(type)))
            using (var cmd = new NpgsqlCommand(command, connection))
            {
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    [CollectionDefinition("testing database")]
    public class DatabaseFixtureCollection : ICollectionFixture<DatabaseFixture>
    {
    }
}
