using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using Xunit;

namespace PostgTest.XUnit.Net
{
    [Collection("PostgreSqlTestDatabase")]
    public abstract class PostgreSqlUnitTest : IDisposable
    {
        protected readonly NpgsqlConnection Connection;
        protected readonly IPostgreSqlTestConfig Config;
        protected readonly NpgsqlTransaction Transaction;

        protected PostgreSqlUnitTest()
        {
            Config = Net.Config.Value;
            Connection = new NpgsqlConnection(Config.GetTestConnectionString());
            EnsureConnectionIsOpen();
            Transaction = Connection.BeginTransaction();
        }

        public void Dispose()
        {
            Transaction.Rollback();
            Transaction.Dispose();
            if (Connection.State == ConnectionState.Open)
            {
                Connection.Close();
            }
            Connection.Dispose();
        }

        protected void Execute(string command)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                EnsureConnectionIsOpen();
                cmd.ExecuteNonQuery();
            }
        }

        protected async Task ExecuteAsync(string command)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        protected void Read(string command, Action<NpgsqlDataReader> read)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                EnsureConnectionIsOpen();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        read(reader);
                    }
                }
            }
        }

        protected async Task ReadAsync(string command, Action<NpgsqlDataReader> read)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                EnsureConnectionIsOpen();
                using (var reader = await cmd.ExecuteReaderAsync() as NpgsqlDataReader)
                {
                    while (reader.Read())
                    {
                        read(reader);
                    }
                }
            }
        }

        protected async Task ReadAsync(string command, Func<NpgsqlDataReader, Task> read)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                EnsureConnectionIsOpen();
                using (var reader = await cmd.ExecuteReaderAsync() as NpgsqlDataReader)
                {
                    while (reader.Read())
                    {
                        await read(reader);
                    }
                }
            }
        }

        private void EnsureConnectionIsOpen()
        {
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }
        }

        private async Task EnsureConnectionIsOpenAsync()
        {
            if (Connection.State != ConnectionState.Open)
            {
                await Connection.OpenAsync();
            }
        }
    }
}
