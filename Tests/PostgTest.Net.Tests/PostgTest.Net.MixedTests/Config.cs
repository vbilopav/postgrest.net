using System.IO;
using PostgTest.Net.Configuration;
using Xunit;

namespace PostgTest.Net.MixedTests
{
    public class Config : PostgTestConfig
    {
        public override int Port => 5433;
        public override string TestDatabase => "mixed_tests";
        public override string TestUser => "mixed_tests_user";

        public override string[] MigrationScripts => new[]
        {
            $@"
            do $$
            begin

                create table companies (
                    id int not null generated always as identity primary key,
                    name varchar(256) not null
                );

                create table employees (
                    id int not null generated always as identity primary key,
                    first_name varchar(256) not null,
                    last_name varchar(256) not null,
                    email varchar(1024) not null,
                    company_id int not null,
                    
                    constraint fk_employees_company_id__companies_id foreign key (company_id)
                    references companies (id)
                    on update no action
                    on delete cascade
                    deferrable initially immediate,

                    unique (email)
                );

                grant select, insert on companies to {this.TestUser};
                grant select, insert on employees to {this.TestUser};
            end
            $$;"
        };
    }

    [CollectionDefinition("PostgreSqlTestDatabase")]
    public class PostgreSqlFixtureCollection : ICollectionFixture<PostgreSqlDatabaseFixture>
    {
    }
}
