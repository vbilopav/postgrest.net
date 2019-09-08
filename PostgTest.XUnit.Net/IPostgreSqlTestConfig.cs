using System.Runtime.CompilerServices;

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
        string CreateTestDatabaseCommand { get; set; }
        string DefaultUserPassword { get; set; }
        string CreateTestUserCommand { get; set; }
        string DropTestDatabaseCommand { get; set; }
        string DropTestUserCommand { get; set; }
    }

    public static class PostgreSqlTestConfigExtensions
    {
        public static string GetDefaultConnectionString(this IPostgreSqlTestConfig config) => 
            $"Server={config.Server};Database={config.DefaultDatabase};Port={config.Port};User Id={config.DefaultUser};Password={config.DefaultUserPassword};";

        public static string GetTestConnectionString(this IPostgreSqlTestConfig config) =>
            string.IsNullOrEmpty(config.TestUser) ? 
                config.GetDefaultConnectionString() :
                $"Server={config.Server};Database={config.TestDatabase};Port={config.Port};User Id={config.TestUser};Password={config.DefaultUserPassword};";
    }
}