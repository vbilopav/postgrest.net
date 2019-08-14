using Npgsql;
using System;
using Xunit;

namespace UnitTests
{
    public class DatabaseFixture : IDisposable
    {
        public DatabaseFixture()
        {
            ExecuteCommand(@"
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
            ");
        }

        public void Dispose()
        {
            ExecuteCommand(@"
            revoke connect on database testing from public;

            select pid, pg_terminate_backend(pid)
            from pg_stat_activity
            where datname = 'testing' AND pid <> pg_backend_pid();

            drop role testing;

            drop database testing;
            ");
        }

        private void ExecuteCommand(string command)
        {
            using (var connection = new NpgsqlConnection(Config.PostgresConnection))
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
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
