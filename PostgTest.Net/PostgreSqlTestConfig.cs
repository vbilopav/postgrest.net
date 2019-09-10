using System;
using Npgsql;

namespace PostgTest.Net
{
    public interface IPostgreSqlTestConfig
    {
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

    public static class PostgreSqlTestConfigExtensions
    {
        public static string GetDefaultConnectionString(this IPostgreSqlTestConfig config) =>
            $"Server={config.Server};Database={config.DefaultDatabase};Port={config.Port};User Id={config.DefaultUser};Password={config.DefaultUserPassword};";

        public static string GetTestConnectionString(this IPostgreSqlTestConfig config) =>
            string.IsNullOrEmpty(config.TestUser) ?
                config.GetDefaultConnectionString() :
                $"Server={config.Server};Database={config.TestDatabase};Port={config.Port};User Id={config.TestUser};Password={config.TestUserPassword};";
    }

    public class PostgreSqlTestConfig : IPostgreSqlTestConfig
    {
        private string createTestDatabaseCmd = null;
        private string createTestUserCmd = null;
        private string dropTestDatabaseCmd = null;
        private string dropTestUserCmd = null;

        public virtual string Server { get; set; } = "localhost";
        public virtual int Port { get; set; } = NpgsqlConnection.DefaultPort;
        public virtual string DefaultDatabase { get; set; } = "postgres";
        public virtual string DefaultUser { get; set; } = "postgres";
        public virtual string DefaultUserPassword { get; set; } = "postgres";
        public virtual string TestDatabase { get; set; } = "postg_test_db";
        public virtual string TestUser { get; set; } = "postg_test_user";
        public virtual string TestUserPassword { get; set; } = "postg_test_user";
        public virtual bool CreateTestDatabase { get; set; } = true;
        public virtual string CreateTestDatabaseCommand
        {
            get
            {
                if (!this.CreateTestDatabase)
                {
                    return null;
                }
                if (createTestDatabaseCmd != null)
                {
                    return createTestDatabaseCmd;
                }
                if (string.IsNullOrEmpty(this.TestDatabase))
                {
                    throw new Exception("Misconfiguration. Check the TestDatabase key.");
                }

                return $@"
                    create database {this.TestDatabase};
                ";
            }
            set => createTestDatabaseCmd = value;
        }
        public virtual string CreateTestUserCommand
        {
            get
            {
                if (createTestUserCmd != null)
                {
                    return createTestUserCmd;
                }
                return string.IsNullOrEmpty(this.TestUser) ? null : $@"
                    create role {this.TestUser} with
                        login
                        nosuperuser
                        nocreatedb
                        nocreaterole
                        noinherit
                        noreplication
                        connection limit -1
                        password '{this.TestUserPassword}';
                ";
            }
            set => createTestUserCmd = value;
        }
        public virtual string DropTestDatabaseCommand
        {
            get
            {
                if (dropTestDatabaseCmd != null)
                {
                    return dropTestDatabaseCmd;
                }
                if (string.IsNullOrEmpty(this.TestDatabase))
                {
                    throw new Exception("Misconfiguration. Check the TestDatabase key.");
                }

                return $@"
                    revoke connect on database {this.TestDatabase} from public;

                    select pid, pg_terminate_backend(pid)
                    from pg_stat_activity
                    where datname = '{this.TestDatabase}' and pid <> pg_backend_pid();

                    drop database {this.TestDatabase};
                ";
            }
            set => dropTestDatabaseCmd = value;
        }
        public virtual string DropTestUserCommand
        {
            get
            {
                if (dropTestUserCmd != null)
                {
                    return dropTestUserCmd;
                }
                return string.IsNullOrEmpty(this.TestUser) ? null : $@"
                    drop role {this.TestUser};
                ";
            }
            set => dropTestUserCmd = value;
        }
    }
}
