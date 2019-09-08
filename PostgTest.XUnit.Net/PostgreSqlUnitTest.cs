using System;
using System.Data;
using Npgsql;
using Xunit;

namespace PostgTest.XUnit.Net
{
    [CollectionDefinition("PostgreSqlTestDatabase")]
    public abstract class PostgreSqlUnitTest : IDisposable
    {
        protected readonly NpgsqlConnection Connection;
        protected readonly IPostgreSqlTestConfig Config;
        protected readonly NpgsqlTransaction Transaction;

        protected PostgreSqlUnitTest()
        {
            Config = Net.Config.Value;
            Connection = new NpgsqlConnection(Config.GetTestConnectionString());
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

        protected void ExecuteCommand(string command)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                if (Connection.State != ConnectionState.Open)
                {
                    Connection.Open();
                }
                cmd.ExecuteNonQuery();
            }
        }
    }
}
