using PostgTest.Net.Configuration;

namespace PostgTest.Net.Migrations
{
    public class ConfigMigration : MigrationBase
    {
        public sealed override string ScriptsDir { get; protected set; }
        public sealed override string[] ScriptFiles { get; protected set; }
        public sealed override string[] Scripts { get; protected set; }

        public ConfigMigration()
        {
            var config = Config.Value;
            ScriptsDir = config.MigrationScriptsDir;
            ScriptFiles = config.MigrationScriptFiles;
            Scripts = config.MigrationScripts;
        }
    }
}