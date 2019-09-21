using System;
using System.Collections.Generic;
using Npgsql;

namespace PostgExecute.Net
{
    public static partial class ConnectionExtensions
    {
        public static IDictionary<string, object> Single(this NpgsqlConnection connection, string command) =>
            new Postg(connection).Single(command);

        public static IDictionary<string, object> Single(this NpgsqlConnection connection, string command, params object[] parameters) =>
            new Postg(connection).Single(command, parameters);

        public static IDictionary<string, object> Single(this NpgsqlConnection connection, string command, params (string name, object value)[] parameters) =>
            new Postg(connection).Single(command, parameters);

        public static IDictionary<string, object> Single(this NpgsqlConnection connection, string command, Action<NpgsqlParameterCollection> parameters) =>
            new Postg(connection).Single(command, parameters);
    }
}