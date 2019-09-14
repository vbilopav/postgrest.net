using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace PostgExecute.Net
{
    public partial class Postg
    {
        public async Task<IPostg> ReadAsync(string command, Action<IDictionary<string, object>> results)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        results(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));
                    }
                    return this;
                }
            }
        }

        public async Task<IPostg> ReadAsync(string command, Action<IDictionary<string, object>> results, params object[] parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                AddParameters(cmd, parameters);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        results(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));
                    }
                    return this;
                }
            }
        }

        public async Task<IPostg> ReadAsync(string command, Action<IDictionary<string, object>> results,
            Action<NpgsqlParameterCollection> parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                AddParameters(cmd, parameters);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        results(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));
                    }

                    return this;
                }
            }
        }

        public async Task<IPostg> ReadAsync(string command, Action<IDictionary<string, object>> results,
            Func<NpgsqlParameterCollection, Task> parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                await AddParametersAsync(cmd, parameters);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        results(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));
                    }

                    return this;
                }
            }
        }

        public async Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, Task> results)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        await results(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));
                    }

                    return this;
                }
            }
        }

        public async Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, Task> results, params object[] parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                AddParameters(cmd, parameters);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        await results(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));
                    }

                    return this;
                }
            }
        }

        public async Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, Task> results, Action<NpgsqlParameterCollection> parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                AddParameters(cmd, parameters);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        await results(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));
                    }

                    return this;
                }
            }
        }

        public async Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, Task> results, Func<NpgsqlParameterCollection, Task> parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                await AddParametersAsync(cmd, parameters);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        await results(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));
                    }

                    return this;
                }
            }
        }


        public async Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, bool> results)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var result = results(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));
                        if (!result)
                        {
                            break;
                        }
                    }
                    return this;
                }
            }
        }

        public async Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, bool> results, params object[] parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                AddParameters(cmd, parameters);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var result = results(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));
                        if (!result)
                        {
                            break;
                        }
                    }
                    return this;
                }
            }
        }

        public async Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, bool> results,
            Action<NpgsqlParameterCollection> parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                AddParameters(cmd, parameters);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var result = results(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));
                        if (!result)
                        {
                            break;
                        }
                    }

                    return this;
                }
            }
        }

        public async Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, bool> results,
            Func<NpgsqlParameterCollection, Task> parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                await AddParametersAsync(cmd, parameters);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var result = results(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));
                        if (!result)
                        {
                            break;
                        }
                    }

                    return this;
                }
            }
        }

        public async Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, Task<bool>> results)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var result = await results(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));
                        if (!result)
                        {
                            break;
                        }
                    }

                    return this;
                }
            }
        }

        public async Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, Task<bool>> results, params object[] parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                AddParameters(cmd, parameters);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var result = await results(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));
                        if (!result)
                        {
                            break;
                        }
                    }

                    return this;
                }
            }
        }

        public async Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, Task<bool>> results, Action<NpgsqlParameterCollection> parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                AddParameters(cmd, parameters);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var result = await results(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));
                        if (!result)
                        {
                            break;
                        }
                    }

                    return this;
                }
            }
        }

        public async Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, Task<bool>> results, Func<NpgsqlParameterCollection, Task> parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                await EnsureConnectionIsOpenAsync();
                await AddParametersAsync(cmd, parameters);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var result = await results(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));
                        if (!result)
                        {
                            break;
                        }
                    }

                    return this;
                }
            }
        }
    }
}
