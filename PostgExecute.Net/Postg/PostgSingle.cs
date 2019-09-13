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
        public IDictionary<string, object> Single(string command)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                EnsureConnectionIsOpen();
                using (var reader = cmd.ExecuteReader())
                {
                    return reader.Read()
                        ? Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue)
                        : new Dictionary<string, object>();
                }
            }
        }

        public IDictionary<string, object> Single(string command, params object[] parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                EnsureConnectionIsOpen();
                AddParameters(cmd, parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    return reader.Read() 
                        ? Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue) 
                        : new Dictionary<string, object>();
                }
            }
        }

        public IDictionary<string, object> Single(string command, Action<NpgsqlParameterCollection> parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                EnsureConnectionIsOpen();
                AddParameters(cmd, parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    return reader.Read() 
                        ? Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue) 
                        : new Dictionary<string, object>();
                }
            }
        }

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
                AddParameters(cmd, parameters);
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
                AddParameters(cmd, parameters);
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
                await AddParametersAsync(cmd, parameters);
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
