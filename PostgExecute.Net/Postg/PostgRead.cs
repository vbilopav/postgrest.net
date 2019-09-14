using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace PostgExecute.Net
{
    public partial class Postg
    {
        public IEnumerable<IDictionary<string, object>> Read(string command)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                EnsureConnectionIsOpen();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return Enumerable.Range(0, reader.FieldCount)
                            .ToDictionary(reader.GetName, reader.GetValue);
                    }
                }
            }
        }

        public IEnumerable<IDictionary<string, object>> Read(string command, params object[] parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                EnsureConnectionIsOpen();
                AddParameters(cmd, parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return Enumerable.Range(0, reader.FieldCount)
                            .ToDictionary(reader.GetName, reader.GetValue);
                    }
                }
            }
        }

        public IEnumerable<IDictionary<string, object>> Read(string command,
            Action<NpgsqlParameterCollection> parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                EnsureConnectionIsOpen();
                AddParameters(cmd, parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return Enumerable.Range(0, reader.FieldCount)
                            .ToDictionary(reader.GetName, reader.GetValue);
                    }
                }
            }
        }

        public IPostg Read(string command, Action<IDictionary<string, object>> results)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                EnsureConnectionIsOpen();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));
                    }
                    return this;
                }
            }
        }

        public IPostg Read(string command, Action<IDictionary<string, object>> results, params object[] parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                EnsureConnectionIsOpen();
                AddParameters(cmd, parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));
                    }

                    return this;
                }
            }
        }

        public IPostg Read(string command, Action<IDictionary<string, object>> results,
            Action<NpgsqlParameterCollection> parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                EnsureConnectionIsOpen();
                AddParameters(cmd, parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results(Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue));
                    }

                    return this;
                }
            }
        }

        public IPostg Read(string command, Func<IDictionary<string, object>, bool> results)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                EnsureConnectionIsOpen();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
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

        public IPostg Read(string command, Func<IDictionary<string, object>, bool> results, params object[] parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                EnsureConnectionIsOpen();
                AddParameters(cmd, parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var result = results(Enumerable.Range(0, reader.FieldCount)
                            .ToDictionary(reader.GetName, reader.GetValue));
                        if (!result)
                        {
                            break;
                        }
                    }

                    return this;
                }
            }
        }

        public IPostg Read(string command, Func<IDictionary<string, object>, bool> results,
                Action<NpgsqlParameterCollection> parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                EnsureConnectionIsOpen();
                AddParameters(cmd, parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var result = results(Enumerable.Range(0, reader.FieldCount)
                            .ToDictionary(reader.GetName, reader.GetValue));
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
