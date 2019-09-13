using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;

namespace PostgExecute.Net
{
    public partial class Postg
    {
        public static IDictionary<string, object> Single(string connection, string command)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                return pg.Single(command);
            }
        }

        public static IDictionary<string, object> Single(string connection, string command, params object[] parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                return pg.Single(command, parameters);
            }
        }

        public static IDictionary<string, object> Single(string connection, string command, Action<NpgsqlParameterCollection> parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                return pg.Single(command, parameters);
            }
        }

        public static async Task<IDictionary<string, object>> SingleAsync(string connection, string command)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                return await pg.SingleAsync(command);
            }
        }

        public static async Task<IDictionary<string, object>> SingleAsync(string connection, string command, params object[] parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                return await pg.SingleAsync(command, parameters);
            }
        }

        public static async Task<IDictionary<string, object>> SingleAsync(string connection, string command, Action<NpgsqlParameterCollection> parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                return await pg.SingleAsync(command, parameters);
            }
        }

        public static async Task<IDictionary<string, object>> SingleAsync(string connection, string command, Func<NpgsqlParameterCollection, Task> parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                return await pg.SingleAsync(command, parameters);
            }
        }
    }
}
