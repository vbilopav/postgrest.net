using System;
using System.Collections.Generic;
using Npgsql;

namespace PostgExecute.Net
{
    public static partial class ConnectionExtensions
    {
        public static NpgsqlConnection Read(this NpgsqlConnection connection, string command, Action<IDictionary<string, object>> results)
        {
            new Postg(connection).Read(command, results);
            return connection;
        }

        public static NpgsqlConnection Read(this NpgsqlConnection connection, string command, Action<IDictionary<string, object>> results, params object[] parameters)
        {
            new Postg(connection).Read(command, results, parameters);
            return connection;
        }

        public static NpgsqlConnection Read(this NpgsqlConnection connection, string command, Action<IDictionary<string, object>> results, params (string name, object value)[] parameters)
        {
            new Postg(connection).Read(command, results, parameters);
            return connection;
        }

        public static NpgsqlConnection Read(this NpgsqlConnection connection, string command, Action<IDictionary<string, object>> results, Action<NpgsqlParameterCollection> parameters)
        {
            new Postg(connection).Read(command, results, parameters);
            return connection;
        }
    }
}