using System;


namespace PostgTest.XUnit.Net
{
    public class PostgreSqlTestConfig : IPostgreSqlTestConfig
    {
        private string createTestDatabase = null;
        private string createTestUser = null;
        private string dropTestDatabase = null;
        private string dropTestUser = null;

        public string Server { get; set; } = "localhost";
        public int Port { get; set; } = 5432;
        public string DefaultDatabase { get; set; } = "postgres";
        public string DefaultUser { get; set; } = "postgres";
        public string DefaultUserPassword { get; set; } = "postgres";
        public string TestDatabase { get; set; } = "postg_test_db";
        public string TestUser { get; set; } = "postg_test_user";
        public string CreateTestDatabase
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
        public string CreateTestUser
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
                        password 'testing';
                ";
            }
            set => createTestUser = value;
        }
        public string DropTestDatabase
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
        public string DropTestUser
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
