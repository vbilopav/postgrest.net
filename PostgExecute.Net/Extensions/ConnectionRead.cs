using System;
using System.Collections.Generic;
using Npgsql;

namespace PostgExecute.Net
{
    public static partial class ConnectionExtensions
    {
        public static IEnumerable<IDictionary<string, object>> Read(this NpgsqlConnection connection, string command) =>
            new Postg(connection).Read(command);

        public static IEnumerable<IDictionary<string, object>> Read(this NpgsqlConnection connection, string command, params object[] parameters) =>
            new Postg(connection).Read(command, parameters);

        public static IEnumerable<IDictionary<string, object>> Read(this NpgsqlConnection connection, string command, params (string name, object value)[] parameters) =>
            new Postg(connection).Read(command, parameters);

        public static IEnumerable<IDictionary<string, object>> Read(this NpgsqlConnection connection, string command, Action<NpgsqlParameterCollection> parameters) =>
            new Postg(connection).Read(command, parameters);
    }
}