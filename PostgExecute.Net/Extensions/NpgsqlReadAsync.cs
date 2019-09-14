using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;

namespace PostgExecute.Net
{
    public static partial class NpgsqlExtensions
    {
        public static async Task<NpgsqlConnection> ReadAsync(this NpgsqlConnection connection, string command,
            Action<IDictionary<string, object>> results)
        {
            await new Postg(connection).ReadAsync(command, results);
            return connection;
        }

        public static async Task<NpgsqlConnection> ReadAsync(this NpgsqlConnection connection, string command,
            Action<IDictionary<string, object>> results, params object[] parameters)
        {
            await new Postg(connection).ReadAsync(command, results, parameters);
            return connection;
        }

        public static async Task<NpgsqlConnection> ReadAsync(this NpgsqlConnection connection, string command, Action<IDictionary<string, object>> results, Action<NpgsqlParameterCollection> parameters)
        {
            await new Postg(connection).ReadAsync(command, results, parameters);
            return connection;
        }

        public static async Task<NpgsqlConnection> ReadAsync(this NpgsqlConnection connection, string command, Action<IDictionary<string, object>> results, Func<NpgsqlParameterCollection, Task> parameters)
        {
            await new Postg(connection).ReadAsync(command, results, parameters);
            return connection;
        }

        public static async Task<NpgsqlConnection> ReadAsync(this NpgsqlConnection connection, string command, Func<IDictionary<string, object>, Task> results)
        {
            await new Postg(connection).ReadAsync(command, results);
            return connection;
        }

        public static async Task<NpgsqlConnection> ReadAsync(this NpgsqlConnection connection, string command, Func<IDictionary<string, object>, Task> results, params object[] parameters)
        {
            await new Postg(connection).ReadAsync(command, results, parameters);
            return connection;
        }

        public static async Task<NpgsqlConnection> ReadAsync(this NpgsqlConnection connection, string command, Func<IDictionary<string, object>, Task> results, Action<NpgsqlParameterCollection> parameters)
        {
            await new Postg(connection).ReadAsync(command, results, parameters);
            return connection;
        }

        public static async Task<NpgsqlConnection> ReadAsync(this NpgsqlConnection connection, string command, Func<IDictionary<string, object>, Task> results, Func<NpgsqlParameterCollection, Task> parameters)
        {
            await new Postg(connection).ReadAsync(command, results, parameters);
            return connection;
        }


        public static async Task<NpgsqlConnection> ReadAsync(this NpgsqlConnection connection, string command, Func<IDictionary<string, object>, bool> results)
        {
            await new Postg(connection).ReadAsync(command, results);
            return connection;
        }

        public static async Task<NpgsqlConnection> ReadAsync(this NpgsqlConnection connection, string command, Func<IDictionary<string, object>, bool> results, params object[] parameters)
        {
            await new Postg(connection).ReadAsync(command, results, parameters);
            return connection;
        }

        public static async Task<NpgsqlConnection> ReadAsync(this NpgsqlConnection connection, string command, Func<IDictionary<string, object>, bool> results, Action<NpgsqlParameterCollection> parameters)
        {
            await new Postg(connection).ReadAsync(command, results, parameters);
            return connection;
        }

        public static async Task<NpgsqlConnection> ReadAsync(this NpgsqlConnection connection, string command, Func<IDictionary<string, object>, bool> results, Func<NpgsqlParameterCollection, Task> parameters)
        {
            await new Postg(connection).ReadAsync(command, results, parameters);
            return connection;
        }

        public static async Task<NpgsqlConnection> ReadAsync(this NpgsqlConnection connection, string command, Func<IDictionary<string, object>, Task<bool>> results)
        {
            await new Postg(connection).ReadAsync(command, results);
            return connection;
        }

        public static async Task<NpgsqlConnection> ReadAsync(this NpgsqlConnection connection, string command, Func<IDictionary<string, object>, Task<bool>> results, params object[] parameters)
        {
            await new Postg(connection).ReadAsync(command, results, parameters);
            return connection;
        }

        public static async Task<NpgsqlConnection> ReadAsync(this NpgsqlConnection connection, string command, Func<IDictionary<string, object>, Task<bool>> results, Action<NpgsqlParameterCollection> parameters)
        {
            await new Postg(connection).ReadAsync(command, results, parameters);
            return connection;
        }

        public static async Task<NpgsqlConnection> ReadAsync(this NpgsqlConnection connection, string command, Func<IDictionary<string, object>, Task<bool>> results, Func<NpgsqlParameterCollection, Task> parameters)
        {
            await new Postg(connection).ReadAsync(command, results, parameters);
            return connection;
        }
    }
}