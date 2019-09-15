namespace PostgTest.Net
{
    public interface IScriptsMigrationFixture
    {
        string ScriptsDir { get; set; }
        string ScriptFile { get; set; }
        string Script { get; set; }
    }
}