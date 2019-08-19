using System.Collections.Generic;

namespace UnitTests
{
    public static class Config
    {
        public const string TestDatabase = "testing";
        public const string TestUser = "testing";
        public const string AdminDatabase = "postgres";
        public const string AdminUser = "postgres";


        public static readonly IDictionary<string, string> Connections = new Dictionary<string, string>()
        {
            { "Postgres", $"Server=localhost;Database={AdminDatabase};Port=5433;User Id={AdminUser};Password={AdminUser};" },
            { "PostgresTesting", $"Server=localhost;Database={TestDatabase};Port=5433;User Id={AdminUser};Password={AdminUser};" },
            { "Testing", $"Server=localhost;Database={TestDatabase};Port=5433;User Id={TestUser};Password={TestUser};" }
        };

        public static string PostgresConnection => Connections["Postgres"];

        public static string PostgresTestingConnection => Connections["PostgresTesting"];

        public static string TestingConnection => Connections["Testing"];

        public static string Connection(ConnectionType type) => Connections[type.ToString()];
 
        public enum ConnectionType
        {
            /// <summary>
            /// postgres (admin) user on postgres database
            /// </summary>
            Postgres,
            /// <summary>
            /// postgres user (admin) on testing database (re-created)
            /// </summary>
            PostgresTesting,
            /// <summary>
            /// testing user (re-created) on testing database (re-created)
            /// </summary>
            Testing
        }
    }
}