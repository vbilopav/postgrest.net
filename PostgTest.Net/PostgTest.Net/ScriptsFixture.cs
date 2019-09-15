using System.IO;
using System.Linq;
using Npgsql;
using PostgExecute.Net;

namespace PostgTest.Net
{
    public abstract class ScriptsFixture
    {
        public virtual string ScriptsDir { get; protected set; }
        public virtual string ScriptFile { get; protected set; }
        public virtual string Script { get; protected set; }

        public virtual void Run(NpgsqlConnection connection)
        {
            RunScriptsDir(connection);
            RunScriptFile(connection);
            RunScript(connection);
        }

        protected virtual void RunScriptsDir(NpgsqlConnection connection)
        {
            if (ScriptsDir == null)
            {
                return;
            }
            foreach (var filename in Directory.EnumerateFiles(ScriptsDir).OrderByDescending(filename => filename))
            {
                connection.Execute(File.ReadAllText(filename));
            }
        }

        protected virtual void RunScriptFile(NpgsqlConnection connection)
        {
            if (ScriptFile == null)
            {
                return;
            }
            connection.Execute(File.ReadAllText(ScriptFile));
        }

        protected virtual void RunScript(NpgsqlConnection connection)
        {
            if (Script == null)
            {
                return;
            }
            connection.Execute(Script);
        }
    }

    public class ConfigScriptsFixture : ScriptsFixture
    {
        public sealed override string ScriptsDir { get; protected set; }
        public sealed override string ScriptFile { get; protected set; }
        public sealed override string Script { get; protected set; }

        public ConfigScriptsFixture()
        {
            var config = Config.Value;
            ScriptsDir = config.MigrationScriptsDir;
            ScriptFile = config.MigrationScriptFile;
            Script = config.MigrationScript;
        }
    }
}