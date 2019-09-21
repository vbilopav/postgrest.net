using System;
using System.Threading.Tasks;
using Npgsql;

namespace PostgExecute.Net
{
    public partial class Postg
    {
        public async Task<IPostg> ExecuteAsync(string command)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return this;
            }
        }

        public async Task<IPostg> ExecuteAsync(string command, params object[] parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                await cmd.AddParameters(parameters).ExecuteNonQueryAsync();
                return this;
            }
        }

        public async Task<IPostg> ExecuteAsync(string command, params (string name, object value)[] parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                await cmd.AddParameters(parameters).ExecuteNonQueryAsync();
                return this;
            }
        }

        public async Task<IPostg> ExecuteAsync(string command, Action<NpgsqlParameterCollection> parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                await cmd.AddParameters(parameters).ExecuteNonQueryAsync();
                return this;
            }
        }

        public async Task<IPostg> ExecuteAsync(string command, Func<NpgsqlParameterCollection, Task> parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                await cmd.AddParametersAsync(parameters);
                await cmd.ExecuteNonQueryAsync();
                return this;
            }
        }
    }
}
