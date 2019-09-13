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
        public static IEnumerable<IDictionary<string, object>> Read(this NpgsqlConnection connection, string command) =>
            new Postg(connection).Read(command);

        public static IEnumerable<IDictionary<string, object>> Read(this NpgsqlConnection connection, string command, params object[] parameters) =>
            new Postg(connection).Read(command, parameters);
        
        public static IEnumerable<IDictionary<string, object>> Read(this NpgsqlConnection connection, string command, Action<NpgsqlParameterCollection> parameters) =>
            new Postg(connection).Read(command, parameters);

        public static async Task ReadAsync(this NpgsqlConnection connection, string command, Action<IDictionary<string, object>> results) =>
            await new Postg(connection).ReadAsync(command, results);

        public static async Task ReadAsync(this NpgsqlConnection connection, string command, Action<IDictionary<string, object>> results, params object[] parameters) =>
            await new Postg(connection).ReadAsync(command, results, parameters);

        public static async Task ReadAsync(this NpgsqlConnection connection, string command, Action<IDictionary<string, object>> results, Action<NpgsqlParameterCollection> parameters) =>
            await new Postg(connection).ReadAsync(command, results, parameters);

        public static async Task ReadAsync(this NpgsqlConnection connection, string command, Action<IDictionary<string, object>> results, Func<NpgsqlParameterCollection, Task> parameters) =>
            await new Postg(connection).ReadAsync(command, results, parameters);

        public static async Task ReadAsync(this NpgsqlConnection connection, string command, Func<IDictionary<string, object>, Task> results) =>
            await new Postg(connection).ReadAsync(command, results);

        public static async Task ReadAsync(this NpgsqlConnection connection, string command, Func<IDictionary<string, object>, Task> results, params object[] parameters) =>
            await new Postg(connection).ReadAsync(command, results, parameters);

        public static async Task ReadAsync(this NpgsqlConnection connection, string command, Func<IDictionary<string, object>, Task> results, Action<NpgsqlParameterCollection> parameters) =>
            await new Postg(connection).ReadAsync(command, results, parameters);

        public static async Task ReadAsync(this NpgsqlConnection connection, string command, Func<IDictionary<string, object>, Task> results, Func<NpgsqlParameterCollection, Task> parameters) =>
            await new Postg(connection).ReadAsync(command, results, parameters);
    }
}