namespace PostgTest.Net.Configuration
{
    public static class PostgTestConfigExtensions
    {
        public static string GetDefaultConnectionString(this IPostgTestConfig config) =>
            $"Server={config.Server};Database={config.DefaultDatabase};Port={config.Port};User Id={config.DefaultUser};Password={config.DefaultUserPassword};";

        public static string GetTestConnectionString(this IPostgTestConfig config) =>
            string.IsNullOrEmpty(config.TestUser) ?
                config.GetDefaultConnectionString() :
                $"Server={config.Server};Database={config.TestDatabase};Port={config.Port};User Id={config.TestUser};Password={config.TestUserPassword};";
    }
}