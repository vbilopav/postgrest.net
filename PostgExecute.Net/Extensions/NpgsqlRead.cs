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
        
        public static IEnumerable<IDictionary<string, object>> Read(this NpgsqlConnection connection, string command, 
            Action<NpgsqlParameterCollection> parameters) =>
            new Postg(connection).Read(command, parameters);

        public static NpgsqlConnection Read(this NpgsqlConnection connection, string command,
            Action<IDictionary<string, object>> results)
        {
            new Postg(connection).Read(command, results);
            return connection;
        }

        public static NpgsqlConnection Read(this NpgsqlConnection connection, string command,
            Action<IDictionary<string, object>> results, params object[] parameters)
        {
            new Postg(connection).Read(command, results, parameters);
            return connection;
        }

        public static NpgsqlConnection Read(this NpgsqlConnection connection, string command,
            Action<IDictionary<string, object>> results, Action<NpgsqlParameterCollection> parameters)
        {
            new Postg(connection).Read(command, results, parameters);
            return connection;
        }

        public static NpgsqlConnection Read(this NpgsqlConnection connection, string command,
            Func<IDictionary<string, object>, bool> results)
        {
            new Postg(connection).Read(command, results);
            return connection;
        }

        public static NpgsqlConnection Read(this NpgsqlConnection connection, string command, 
            Func<IDictionary<string, object>, bool> results, params object[] parameters)
        {
            new Postg(connection).Read(command, results, parameters);
            return connection;
        }

        public static NpgsqlConnection Read(this NpgsqlConnection connection, string command, 
            Func<IDictionary<string, object>, bool> results, Action<NpgsqlParameterCollection> parameters)
        {
            new Postg(connection).Read(command, results, parameters);
            return connection;
        }
    }
}