namespace PostgTest.Net
{
    public interface IPostgreSqlTestConfig
    {
        /*
         * Address of PostgreSql server where tests will be conducted
         * Default value is `localhost`
         */
        string Server { get; set; }
        int Port { get; set; }
        string DefaultDatabase { get; set; }
        string DefaultUser { get; set; }
        string DefaultUserPassword { get; set; }
        string TestDatabase { get; set; }
        string TestUser { get; set; }
        string TestUserPassword { get; set; }
        bool CreateTestDatabase { get; set; }
        string CreateTestDatabaseCommand { get; set; }
        
        string CreateTestUserCommand { get; set; }
        string DropTestDatabaseCommand { get; set; }
        string DropTestUserCommand { get; set; }
    }
}