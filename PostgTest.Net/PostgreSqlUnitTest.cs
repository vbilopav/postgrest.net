using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace PostgTest.Net
{
    public abstract class PostgreSqlUnitTest : IDisposable
    {
        private readonly IPostgreSqlFixture fixture;
        protected readonly IPostgreSqlTestConfig Config;
        protected readonly NpgsqlTransaction Transaction;

        protected PostgreSqlUnitTest(IPostgreSqlFixture fixture)
        {
            this.fixture = fixture;
            Config = Net.Config.Value;
            EnsureConnectionIsOpen();
            Transaction = Connection.BeginTransaction();
        }

        protected NpgsqlConnection Connection => fixture.Connection;

        public void Dispose()
        {
            if (!Transaction.IsCompleted)
            {
                Transaction.Rollback();
            }
            Transaction.Dispose();
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

        protected IEnumerable<IDictionary<string, object>> Read(string command)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                EnsureConnectionIsOpen();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue);
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
