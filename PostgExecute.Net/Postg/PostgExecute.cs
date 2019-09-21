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
                cmd.AddParameters(parameters).ExecuteNonQuery();
                return this;
            }
        }

        public IPostg Execute(string command, params (string name, object value)[] parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                EnsureConnectionIsOpen();
                cmd.AddParameters(parameters).ExecuteNonQuery();
                return this;
            }
        }

        public IPostg Execute(string command, Action<NpgsqlParameterCollection> parameters)
        {
            using (var cmd = new NpgsqlCommand(command, Connection))
            {
                EnsureConnectionIsOpen();
                cmd.AddParameters(parameters).ExecuteNonQuery();
                return this;
            }
        }
    }
}
