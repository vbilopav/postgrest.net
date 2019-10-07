using System.IO;
using System.Linq;
using Norm.Extensions;
using Npgsql;

namespace PostgTest.Net.Migrations
{
    public abstract class MigrationBase
    {
        public virtual string ScriptsDir { get; protected set; }
        public virtual string[] ScriptFiles { get; protected set; }
        public virtual string[] Scripts { get; protected set; }

        public IFixture Fixture { get; protected set; }

        public virtual void Run(NpgsqlConnection connection, IFixture fixture)
        {
            Fixture = fixture;
            OnMigrationStart();
            RunScriptsDir(connection);
            RunScriptFile(connection);
            RunScript(connection);
            OnMigrationEnd();
        }

        public virtual void OnMigrationStart()
        {
        }

        public virtual void OnMigrationEnd()
        {
        }

        protected virtual void RunScriptsDir(NpgsqlConnection connection)
        {
            if (ScriptsDir == null)
            {
                return;
            }
            foreach (var filename in Directory.EnumerateFiles(ScriptsDir).OrderBy(filename => filename))
            {
                connection.Execute(File.ReadAllText(filename));
            }
        }

        protected virtual void RunScriptFile(NpgsqlConnection connection)
        {
            if (ScriptFiles == null)
            {
                return;
            }
            foreach (var filename in ScriptFiles)
            {
                connection.Execute(File.ReadAllText(filename));
            }
        }

        protected virtual void RunScript(NpgsqlConnection connection)
        {
            if (Scripts == null)
            {
                return;
            }
            foreach (var script in Scripts)
            {
                connection.Execute(script);
            }
        }
    }
}