using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Npgsql;

namespace PostgExecute.Net
{
    public partial class Postg : IDisposable, IPostg
    {
        public NpgsqlConnection Connection { get; }

        public Postg(NpgsqlConnection connection)
        {
            Connection = connection;
        }

        public void Dispose()
        {
            if (Connection.State == ConnectionState.Open)
            {
                Connection.Close();
            }
            Connection?.Dispose();
        }

        private void EnsureConnectionIsOpen()
        {
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }
        }

        private async Task EnsureConnectionIsOpenAsync()
        {
            if (Connection.State != ConnectionState.Open)
            {
                await Connection.OpenAsync();
            }
        }

        private static readonly char[] NonCharacters = 
            {' ', '\n', '\r', ',', ';', ':', '-', '!', '"', '#', '$', '%', '&', '/', '(', ')', '=', '?', '*', '\\', '.'};

        private const string ParamPrefix = "@";

        private static void AddParameters(NpgsqlCommand cmd, object[] parameters)
        {
            var command = cmd.CommandText;
            var paramIndex = 0;
            for (var index = 0;; index += ParamPrefix.Length)
            {
                index = command.IndexOf(ParamPrefix, index, StringComparison.Ordinal);
                if (index == -1)
                    break;
                index++;
                var endOf = command.IndexOfAny(NonCharacters, index);
                var name = endOf == -1 ? command.Substring(index) : command.Substring(index, endOf - index);
                cmd.Parameters.Add(new NpgsqlParameter(name, parameters[paramIndex++]));
                index = endOf;
            }
        }

        private static void AddParameters(NpgsqlCommand cmd, Action<NpgsqlParameterCollection> parameters) =>
            parameters?.Invoke(cmd.Parameters);

        private static async Task AddParametersAsync(NpgsqlCommand cmd, Func<NpgsqlParameterCollection, Task> parameters)
        {
            if (parameters != null)
            {
                await parameters.Invoke(cmd.Parameters);
            }
        }
    }
}
