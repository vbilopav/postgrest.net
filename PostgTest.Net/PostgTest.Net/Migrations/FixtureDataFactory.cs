using Npgsql;

namespace PostgTest.Net
{
    public class FixtureDataFactory
    {
        private readonly NpgsqlConnection connection;
        private readonly NpgsqlConnection elevated;

        public FixtureDataFactory(NpgsqlConnection connection, NpgsqlConnection elevated = null)
        {
            this.connection = connection;
            this.elevated = elevated ?? connection;
        }

        public FixtureDataFactory Insert(string table)
        {
            return this;
        }

        public FixtureDataFactory ForField(string name, object value)
        {
            return this;
        }

        public FixtureDataFactory _(string name, object value) => this.ForField(name, value);

        public FixtureDataFactory ForFields(params (string name, object value)[] parameters)
        {
            return this;
        }

        static void Test()
        {
            var inst = new FixtureDataFactory(null);
            inst.ForFields(("bla", 1), ("bla2", 2));
        }
    }

    
}