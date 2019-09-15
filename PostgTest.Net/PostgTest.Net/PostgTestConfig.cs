using System;
using Npgsql;

namespace PostgTest.Net
{
    public class PostgTestConfig : IPostgTestConfig
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

        public virtual bool DisableFixtureTransaction { get; set; } = false;

        public virtual string ScriptsDir { get; set; } = null;

        public virtual string ScriptFile { get; set; } = null;

        public virtual string Script { get; set; } = null;
    }
}
