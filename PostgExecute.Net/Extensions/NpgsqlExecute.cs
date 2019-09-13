using System;
using System.Threading.Tasks;
using Npgsql;

namespace PostgExecute.Net
{
    public static partial class NpgsqlExtensions
    {
        public static NpgsqlConnection Execute(this NpgsqlConnection connection, string command)
        {
            new Postg(connection).Execute(command);
            return connection;
        }

        public static NpgsqlConnection Execute(this NpgsqlConnection connection, string command, params object[] parameters)
        {
            new Postg(connection).Execute(command, parameters);
            return connection;
        }

        public static NpgsqlConnection Execute(this NpgsqlConnection connection, string command, Action<NpgsqlParameterCollection> parameters)
        {
            new Postg(connection).Execute(command, parameters);
            return connection;
        }

        public static async Task<NpgsqlConnection> ExecuteAsync(this NpgsqlConnection connection, string command)
        {
            await new Postg(connection).ExecuteAsync(command);
            return connection;
        }

        public static async Task<NpgsqlConnection> ExecuteAsync(this NpgsqlConnection connection, string command, params object[] parameters)
        {
            await new Postg(connection).ExecuteAsync(command, parameters);
            return connection;
        }

        public static async Task<NpgsqlConnection> ExecuteAsync(this NpgsqlConnection connection, string command, Action<NpgsqlParameterCollection> parameters)
        {
            await new Postg(connection).ExecuteAsync(command, parameters);
            return connection;
        }

        public static async Task<NpgsqlConnection> ExecuteAsync(this NpgsqlConnection connection, string command, Func<NpgsqlParameterCollection, Task> parameters)
        {
            await new Postg(connection).ExecuteAsync(command, parameters);
            return connection;
        }
    }
}
