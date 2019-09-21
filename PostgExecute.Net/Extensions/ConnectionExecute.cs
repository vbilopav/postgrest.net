using System;
using Npgsql;

namespace PostgExecute.Net
{
    public static partial class ConnectionExtensions
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

        public static NpgsqlConnection Execute(this NpgsqlConnection connection, string command, params (string name, object value)[] parameters)
        {
            new Postg(connection).Execute(command, parameters);
            return connection;
        }

        public static NpgsqlConnection Execute(this NpgsqlConnection connection, string command, Action<NpgsqlParameterCollection> parameters)
        {
            new Postg(connection).Execute(command, parameters);
            return connection;
        }
    }
}
