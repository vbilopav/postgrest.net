using System;
using System.Threading.Tasks;
using Npgsql;

namespace PostgExecute.Net
{
    public partial class Postg
    {
        public IPostg Execute(string command)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                EnsureConnectionIsOpen();
                cmd.ExecuteNonQuery();
                return this;
            }
        }

        public IPostg Execute(string command, params object[] parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                EnsureConnectionIsOpen();
                AddParameters(cmd, parameters);
                cmd.ExecuteNonQuery();
                return this;
            }
        }

        public IPostg Execute(string command, Action<NpgsqlParameterCollection> parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                EnsureConnectionIsOpen();
                AddParameters(cmd, parameters);
                cmd.ExecuteNonQuery();
                return this;
            }
        }

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
                AddParameters(cmd, parameters);
                await cmd.ExecuteNonQueryAsync();
                return this;
            }
        }

        public async Task<IPostg> ExecuteAsync(string command, Action<NpgsqlParameterCollection> parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                AddParameters(cmd, parameters);
                await cmd.ExecuteNonQueryAsync();
                return this;
            }
        }

        public async Task<IPostg> ExecuteAsync(string command, Func<NpgsqlParameterCollection, Task> parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                await AddParametersAsync(cmd, parameters);
                await cmd.ExecuteNonQueryAsync();
                return this;
            }
        }
    }
}
