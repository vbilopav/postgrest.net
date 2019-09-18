﻿using System.IO;
using System.Linq;
using Npgsql;
using PostgExecute.Net;

namespace PostgTest.Net
{
    public abstract class ScriptsFixture
    {
        public virtual string ScriptsDir { get; protected set; }
        public virtual string[] ScriptFiles { get; protected set; }
        public virtual string[] Scripts { get; protected set; }

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

    public class NullScriptsFixture : ScriptsFixture { }

    public class ConfigScriptsFixture : ScriptsFixture
    {
        public sealed override string ScriptsDir { get; protected set; }
        public sealed override string[] ScriptFiles { get; protected set; }
        public sealed override string[] Scripts { get; protected set; }

        public ConfigScriptsFixture()
        {
            var config = Config.Value;
            ScriptsDir = config.MigrationScriptsDir;
            ScriptFiles = config.MigrationScriptFiles;
            Scripts = config.MigrationScripts;
        }
    }
}