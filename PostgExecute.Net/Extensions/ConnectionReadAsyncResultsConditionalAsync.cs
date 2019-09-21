﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;

namespace PostgExecute.Net
{
    public static partial class ConnectionExtensions
    {
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

        public static async Task<NpgsqlConnection> ReadAsync(this NpgsqlConnection connection, string command, Func<IDictionary<string, object>, Task<bool>> results, params (string name, object value)[] parameters)
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