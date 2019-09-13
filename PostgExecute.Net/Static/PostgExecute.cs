using System;
using System.Threading.Tasks;
using Npgsql;

namespace PostgExecute.Net
{
    public partial class Postg
    {
        public static void Execute(string connection, string command)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                pg.Execute(command);
            }
        }

        public static void Execute(string connection, string command, params object[] parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                pg.Execute(command, parameters);
            }
        }

        public static void Execute(string connection, string command, Action<NpgsqlParameterCollection> parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                pg.Execute(command, parameters);

            }
        }

        public static async Task ExecuteAsync(string connection, string command)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                await pg.ExecuteAsync(command);
            }
        }

        public static async Task ExecuteAsync(string connection, string command, params object[] parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                await pg.ExecuteAsync(command, parameters);
            }
        }

        public static async Task ExecuteAsync(string connection, string command, Action<NpgsqlParameterCollection> parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                await pg.ExecuteAsync(command, parameters);
            }
        }

        public static async Task ExecuteAsync(string connection, string command, Func<NpgsqlParameterCollection, Task> parameters)
        {
            using (var pg = new Postg(new NpgsqlConnection(connection)))
            {
                await pg.ExecuteAsync(command, parameters);
            }
        }
    }
}
