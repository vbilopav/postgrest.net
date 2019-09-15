namespace PostgTest.Net
{
    public class ScriptsMigrationFixture : IScriptsMigrationFixture
    {
        public string ScriptsDir { get; set; }
        public string ScriptFile { get; set; }
        public string Script { get; set; }
    }
}