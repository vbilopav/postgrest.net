using System.Collections.Generic;

namespace UnitTests
{
    public static class Config
    {
        public const string TestDatabase = "testing";
        public const string TestUser = "testing";

        public static readonly IDictionary<string, string> Connections = new Dictionary<string, string>()
        {
            { "Postgres", "Server=localhost;Database=postgres;Port=5433;User Id=postgres;Password=postgres;" },
            { "PostgresTesting", $"Server=localhost;Database={TestDatabase};Port=5433;User Id=postgres;Password=postgres;" },
            { "Testing", $"Server=localhost;Database={TestDatabase};Port=5433;User Id={TestUser};Password={TestUser};" }
        };

        public static string PostgresConnection { get => Connections["Postgres"]; }

        public static string PostgresTestingConnection { get => Connections["PostgresTesting"]; }

        public static string TestingConnection { get => Connections["Testing"]; }

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