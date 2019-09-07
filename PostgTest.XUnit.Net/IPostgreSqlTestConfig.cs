namespace PostgTest.XUnit.Net
{
    public interface IPostgreSqlTestConfig
    {
        string Server { get; set; }
        int Port { get; set; }
        string DefaultDatabase { get; set; }
        string DefaultUser { get; set; }
        string TestDatabase { get; set; }
        string TestUser { get; set; }
        string CreateTestDatabase { get; set; }
        string DefaultUserPassword { get; set; }
        string CreateTestUser { get; set; }
        string DropTestDatabase { get; set; }
        string DropTestUser { get; set; }
    }
}