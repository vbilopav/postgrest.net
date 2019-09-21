using System;
using System.Threading.Tasks;
using Npgsql;

namespace PostgExecute.Net
{
    public static class CommandExtensions
    {
        private static readonly char[] NonCharacters =
            {' ', '\n', '\r', ',', ';', ':', '-', '!', '"', '#', '$', '%', '&', '/', '(', ')', '=', '?', '*', '\\', '.'};

        private const string ParamPrefix = "@";

        public static NpgsqlCommand AddParameters(this NpgsqlCommand cmd, params object[] parameters)
        {
            var command = cmd.CommandText;
            var paramIndex = 0;
            for (var index = 0; ; index += ParamPrefix.Length)
            {
                index = command.IndexOf(ParamPrefix, index, StringComparison.Ordinal);
                if (index == -1)
                    break;
                index++;
                var endOf = command.IndexOfAny(NonCharacters, index);
                var name = endOf == -1 ? command.Substring(index) : command.Substring(index, endOf - index);
                cmd.Parameters.Add(new NpgsqlParameter(name, parameters[paramIndex++]));
                if (endOf == -1)
                    break;
                index = endOf;
            }

            return cmd;
        }

        public static NpgsqlCommand AddParameters(this NpgsqlCommand cmd, params (string name, object value)[] parameters)
        {
            foreach (var (name, value) in parameters)
            {
                cmd.Parameters.AddWithValue(name, value);
            }

            return cmd;
        }

        public static NpgsqlCommand AddParameters(this NpgsqlCommand cmd, Action<NpgsqlParameterCollection> parameters)
        {
            parameters?.Invoke(cmd.Parameters);
            return cmd;
        }

        public static async Task<NpgsqlCommand> AddParametersAsync(this NpgsqlCommand cmd, Func<NpgsqlParameterCollection, Task> parameters)
        {
            if (parameters != null)
            {
                await parameters.Invoke(cmd.Parameters);
            }

            return cmd;
        }
    }
}
