using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace PostgExecute.Net
{
    public partial class Postg
    {
        public async Task<IDictionary<string, object>> SingleAsync(string command)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                using (var reader = cmd.ExecuteReader())
                {
                    return await reader.ReadAsync() 
                        ? Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue) 
                        : new Dictionary<string, object>();
                }
            }
        }

        public async Task<IDictionary<string, object>> SingleAsync(string command, params object[] parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                cmd.AddParameters(parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    return await reader.ReadAsync() 
                        ? Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue) 
                        : new Dictionary<string, object>();
                }
            }
        }

        public async Task<IDictionary<string, object>> SingleAsync(string command, params (string name, object value)[] parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                cmd.AddParameters(parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    return await reader.ReadAsync()
                        ? Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue)
                        : new Dictionary<string, object>();
                }
            }
        }

        public async Task<IDictionary<string, object>> SingleAsync(string command,
            Action<NpgsqlParameterCollection> parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                cmd.AddParameters(parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    return await reader.ReadAsync() 
                        ? Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue) 
                        : new Dictionary<string, object>();
                }
            }
        }

        public async Task<IDictionary<string, object>> SingleAsync(string command,
            Func<NpgsqlParameterCollection, Task> parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                await cmd.AddParametersAsync(parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    return await reader.ReadAsync() 
                        ? Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue) 
                        : new Dictionary<string, object>();
                }
            }
        }
    }
}
