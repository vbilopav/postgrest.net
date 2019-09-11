namespace PostgTest.Net
{
    public interface IPostgTestConfig
    {
        /// <summary>
        /// Address of PostgreSql server where tests will be conducted.
        /// Default value is `localhost`
        /// </summary>
        string Server { get; set; }

        /// <summary>
        /// Port of PostgreSql server where tests will be conducted.
        /// Default value is `NpgsqlConnection.DefaultPort` which is 5432
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// Name of database which default connection is using to connect.
        /// Default value is `postgres`
        /// Default connection is used to create and drop test database and user (if configured) and/or insert test data (if configured).
        /// </summary>
        string DefaultDatabase { get; set; }

        /// <summary>
        /// Name of user which default connection is using to connect.
        /// Default value is `postgres`
        /// Default connection is used to create and drop test database and user (if configured) and/or insert test data (if configured).
        /// </summary>
        string DefaultUser { get; set; }

        /// <summary>
        /// Password for user which default connection is using to connect.
        /// Default value is `postgres`
        /// Default connection is used to create and drop test database and user (if configured) and/or insert test data (if configured).
        /// </summary>
        string DefaultUserPassword { get; set; }

        /// <summary>
        /// Name of database where tests will be conducted.
        /// Default values is `postg_test_db`
        /// </summary>
        string TestDatabase { get; set; }

        /// <summary>
        /// Name of database user role under which tests will be conducted.
        /// Default values is `postg_test_user`
        /// </summary>
        string TestUser { get; set; }

        /// <summary>
        /// Password of database user role under which tests will be conducted.
        /// Default values is `postg_test_user`
        /// </summary>
        string TestUserPassword { get; set; }

        /// <summary>
        /// Should test database be re-created before any testing?
        /// Set this to `false` if you want to test on specific database of your choice.
        /// Default values is `true`
        /// </summary>
        bool CreateTestDatabase { get; set; }

        /// <summary>
        /// Command that will be executed when creating test database
        /// Default value is `create database {this.TestDatabase};`
        /// </summary>
        string CreateTestDatabaseCommand { get; set; }

        /// <summary>
        /// Command that will be executed when user role for test database is being created
        /// Default value is:
        /// ```
        /// create role {this.TestUser} with
        ///     login
        ///     nosuperuser
        ///     nocreatedb
        ///     nocreaterole
        ///     noinherit
        ///     noreplication
        ///     connection limit -1
        ///     password '{this.TestUserPassword}'
        /// ```
        /// </summary>
        string CreateTestUserCommand { get; set; }

        /// <summary>
        /// Command that will executed when dropping the test database 
        /// Default value is
        /// ```
        /// revoke connect on database {this.TestDatabase }
        /// from public;
        /// select pid, pg_terminate_backend(pid)
        /// from pg_stat_activity
        /// where datname = '{this.TestDatabase}' and pid<> pg_backend_pid();
        /// drop database {this.TestDatabase };
        /// ```
        /// </summary>
        string DropTestDatabaseCommand { get; set; }

        /// <summary>
        /// Command that will be executed when dropping test role 
        /// Default value is `drop role {this.TestUser};`
        /// </summary>
        string DropTestUserCommand { get; set; }
    }
}