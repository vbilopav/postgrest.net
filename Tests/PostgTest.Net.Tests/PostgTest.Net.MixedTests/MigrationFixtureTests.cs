using System.Linq;
using Norm.Extensions;
using PostgTest.Net.Migrations;
using Xunit;


namespace PostgTest.Net.MixedTests
{
    public class TestMigration : MigrationBase
    {
        public override string[] ScriptFiles => new[] { "../../../../Scripts/insert_test_data.sql" };

        public override void OnMigrationEnd()
        {
            this.Fixture.TestConnection.Execute("");
        }
    }

    [Collection("PostgreSqlTestDatabase")]
    public class TestScriptFixtureData : IClassFixture<PostgreSqlTestFixture<TestMigration>>
    {
        private readonly PostgreSqlTestFixture<TestMigration> fixture;

        public TestScriptFixtureData(PostgreSqlTestFixture<TestMigration> fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void TestCompaniesTable()
        {
            var read = fixture.TestConnection.Read("select * from companies").SelectDictionaries().ToList();
            
            Assert.Single(read);
            Assert.Equal("vb-software", read.First()["name"]);
        }

        [Fact]
        public void TestEmployeesTable()
        {
            var list = fixture
                .TestConnection
                .Read("select * from employees where company_id = (select id from companies limit 1)")
                .SelectDictionaries()
                .ToList();

            Assert.Equal(2, list.Count);
            Assert.Equal("Vedran", list[0]["first_name"]);
            Assert.Equal("Bilopavlović", list[0]["last_name"]);
            Assert.Equal("vbilopav@gmail.com", list[0]["email"]);

            Assert.Equal("Floki", list[1]["first_name"]);
            Assert.Equal("The Dog", list[1]["last_name"]);
            Assert.Equal("vbilopav+floki@gmail.com", list[1]["email"]);
        }
    }
}
