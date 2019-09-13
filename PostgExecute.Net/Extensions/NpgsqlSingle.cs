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
        public static IDictionary<string, object> Single(this NpgsqlConnection connection, string command) =>
            new Postg(connection).Single(command);

        public static IDictionary<string, object> Single(this NpgsqlConnection connection, string command, params object[] parameters) =>
            new Postg(connection).Single(command, parameters);

        public static IDictionary<string, object> Single(this NpgsqlConnection connection, string command, Action<NpgsqlParameterCollection> parameters) =>
            new Postg(connection).Single(command, parameters);

        public static async Task<IDictionary<string, object>> SingleAsync(this NpgsqlConnection connection, string command) =>
            await new Postg(connection).SingleAsync(command);

        public static async Task<IDictionary<string, object>> SingleAsync(this NpgsqlConnection connection, string command, params object[] parameters) =>
            await new Postg(connection).SingleAsync(command, parameters);

        public static async Task<IDictionary<string, object>> SingleAsync(this NpgsqlConnection connection, string command, Action<NpgsqlParameterCollection> parameters) =>
            await new Postg(connection).SingleAsync(command, parameters);

        public static async Task<IDictionary<string, object>> SingleAsync(this NpgsqlConnection connection, string command, Func<NpgsqlParameterCollection, Task> parameters) =>
            await new Postg(connection).SingleAsync(command, parameters);
    }
}