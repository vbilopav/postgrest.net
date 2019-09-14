using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;

namespace PostgExecute.Net
{
    public partial class Postg
    {
        public static async Task ReadAsync(string connection, string command, Action<IDictionary<string, object>> results)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                await pg.ReadAsync(command, results);
            }
        }

        public static async Task ReadAsync(string connection, string command, Action<IDictionary<string, object>> results, params object[] parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                await pg.ReadAsync(command, results, parameters);
            }
        }

        public static async Task ReadAsync(string connection, string command, Action<IDictionary<string, object>> results, Action<NpgsqlParameterCollection> parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                await pg.ReadAsync(command, results, parameters);
            }
        }

        public static async Task ReadAsync(string connection, string command, Action<IDictionary<string, object>> results, Func<NpgsqlParameterCollection, Task> parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                await pg.ReadAsync(command, results, parameters);
            }
        }

        public static async Task ReadAsync(string connection, string command, Func<IDictionary<string, object>, Task> results)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                await pg.ReadAsync(command, results);
            }
        }

        public static async Task ReadAsync(string connection, string command, Func<IDictionary<string, object>, Task> results, params object[] parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                await pg.ReadAsync(command, results, parameters);
            }
        }

        public static async Task ReadAsync(string connection, string command, Func<IDictionary<string, object>, Task> results, Action<NpgsqlParameterCollection> parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                await pg.ReadAsync(command, results, parameters);
            }
        }

        public static async Task ReadAsync(string connection, string command, Func<IDictionary<string, object>, Task> results, Func<NpgsqlParameterCollection, Task> parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                await pg.ReadAsync(command, results, parameters);
            }
        }

        public static async Task ReadAsync(string connection, string command, Func<IDictionary<string, object>, bool> results)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                await pg.ReadAsync(command, results);
            }
        }

        public static async Task ReadAsync(string connection, string command, Func<IDictionary<string, object>, bool> results, params object[] parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                await pg.ReadAsync(command, results, parameters);
            }
        }

        public static async Task ReadAsync(string connection, string command, Func<IDictionary<string, object>, bool> results, Action<NpgsqlParameterCollection> parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                await pg.ReadAsync(command, results, parameters);
            }
        }

        public static async Task ReadAsync(string connection, string command, Func<IDictionary<string, object>, bool> results, Func<NpgsqlParameterCollection, Task> parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                await pg.ReadAsync(command, results, parameters);
            }
        }

        public static async Task ReadAsync(string connection, string command, Func<IDictionary<string, object>, Task<bool>> results)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                await pg.ReadAsync(command, results);
            }
        }

        public static async Task ReadAsync(string connection, string command, Func<IDictionary<string, object>, Task<bool>> results, params object[] parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                await pg.ReadAsync(command, results, parameters);
            }
        }

        public static async Task ReadAsync(string connection, string command, Func<IDictionary<string, object>, Task<bool>> results, Action<NpgsqlParameterCollection> parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                await pg.ReadAsync(command, results, parameters);
            }
        }

        public static async Task ReadAsync(string connection, string command, Func<IDictionary<string, object>, Task<bool>> results, Func<NpgsqlParameterCollection, Task> parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                await pg.ReadAsync(command, results, parameters);
            }
        }
    }
}
