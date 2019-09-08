using System;


namespace PostgTest.XUnit.Net
{
    public class PostgreSqlTestConfig : IPostgreSqlTestConfig
    {
        private string createTestDatabase = null;
        private string createTestUser = null;
        private string dropTestDatabase = null;
        private string dropTestUser = null;

        public virtual string Server { get; set; } = "localhost";
        public virtual int Port { get; set; } = 5432;
        public virtual string DefaultDatabase { get; set; } = "postgres";
        public virtual string DefaultUser { get; set; } = "postgres";
        public virtual string DefaultUserPassword { get; set; } = "postgres";
        public virtual string TestDatabase { get; set; } = "postg_test_db";
        public virtual string TestUser { get; set; } = "postg_test_user";
        public virtual string CreateTestDatabaseCommand
        {
            get
            {
                if (createTestDatabase != null)
                {
                    return createTestDatabase;
                }
                if (string.IsNullOrEmpty(this.TestDatabase))
                {
                    throw new Exception("Misconfiguration. Check the TestDatabase key.");
                }

                return $@"
                    create database {this.TestDatabase};
                ";
            }
            set => createTestDatabase = value;
        }
        public virtual string CreateTestUserCommand
        {
            get
            {
                if (createTestUser != null)
                {
                    return createTestUser;
                }
                return string.IsNullOrEmpty(this.TestUser) ? null : $@"
                    create role testing with
                        {this.TestUser}
                        nosuperuser
                        nocreatedb
                        nocreaterole
                        noinherit
                        noreplication
                        connection limit -1
                        password '{this.TestUser}';
                ";
            }
            set => createTestUser = value;
        }
        public virtual string DropTestDatabaseCommand
        {
            get
            {
                if (dropTestDatabase != null)
                {
                    return dropTestDatabase;
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
            set => dropTestDatabase = value;
        }
        public virtual string DropTestUserCommand
        {
            get
            {
                if (dropTestUser != null)
                {
                    return dropTestUser;
                }
                return string.IsNullOrEmpty(this.TestUser) ? null : $@"
                    drop role {this.TestUser};
                ";
            }
            set => dropTestUser = value;
        }
    }
}
