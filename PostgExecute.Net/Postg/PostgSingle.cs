using System;
using System.Collections.Generic;
using System.Linq;
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
                cmd.AddParameters(parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    return reader.Read() 
                        ? Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue) 
                        : new Dictionary<string, object>();
                }
            }
        }

        public IDictionary<string, object> Single(string command, params (string name, object value)[] parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                EnsureConnectionIsOpen();
                cmd.AddParameters(parameters);
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
                cmd.AddParameters(parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    return reader.Read() 
                        ? Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue) 
                        : new Dictionary<string, object>();
                }
            }
        }
    }
}
