using System.Linq;
using PostgExecute.Net;
using Xunit;


namespace PostgTest.Net.ClassFixturesTests
{
    public class TestMigration : MigrationBase
    {
        public override string ScriptsDir => "../../../../Scripts/MigrationFiles";
        public override string[] ScriptFiles => new[] { "../../../../Scripts/insert_test_data.sql" };
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
            var read = fixture.TestConnection.Read("select * from companies").ToList();
            Assert.Single(read);
            Assert.Equal("vb-software", read.First()["name"]);
        }

        [Fact]
        public void TestEmployeesTable()
        {
            var read = fixture.TestConnection.Read("select * from employees where company_id = (select id from companies limit 1)").ToList();
            Assert.Equal(2, read.Count);

            Assert.Equal("Vedran", read[0]["first_name"]);
            Assert.Equal("Bilopavlović", read[0]["last_name"]);
            Assert.Equal("vbilopav@gmail.com", read[0]["email"]);

            Assert.Equal("Floki", read[1]["first_name"]);
            Assert.Equal("The Dog", read[1]["last_name"]);
            Assert.Equal("vbilopav+floki@gmail.com", read[1]["email"]);
        }
    }
}
