using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;

namespace PostgExecute.Net
{
    public static partial class ConnectionExtensions
    {
        public static async Task<IDictionary<string, object>> SingleAsync(this NpgsqlConnection connection, string command) =>
            await new Postg(connection).SingleAsync(command);

        public static async Task<IDictionary<string, object>> SingleAsync(this NpgsqlConnection connection, string command, params object[] parameters) =>
            await new Postg(connection).SingleAsync(command, parameters);

        public static async Task<IDictionary<string, object>> SingleAsync(this NpgsqlConnection connection, string command, params (string name, object value)[] parameters) =>
            await new Postg(connection).SingleAsync(command, parameters);

        public static async Task<IDictionary<string, object>> SingleAsync(this NpgsqlConnection connection, string command, Action<NpgsqlParameterCollection> parameters) =>
            await new Postg(connection).SingleAsync(command, parameters);

        public static async Task<IDictionary<string, object>> SingleAsync(this NpgsqlConnection connection, string command, Func<NpgsqlParameterCollection, Task> parameters) =>
            await new Postg(connection).SingleAsync(command, parameters);
    }
}