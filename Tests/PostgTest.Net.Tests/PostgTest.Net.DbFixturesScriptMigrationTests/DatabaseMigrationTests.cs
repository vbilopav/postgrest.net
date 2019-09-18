using System.Linq;
using Npgsql;
using PostgExecute.Net;
using Xunit;

namespace PostgTest.Net.DbFixturesScriptMigrationTests
{
    [Collection("PostgreSqlTestDatabase")]
    public class DatabaseMigrationTests : IClassFixture<PostgreSqlTestFixture>
    {
        private readonly PostgreSqlTestFixture fixture;
        private NpgsqlConnection Connection => fixture.DefaultConnection;

        public DatabaseMigrationTests(PostgreSqlTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void TestCompaniesTable()
        {
            var read = Connection.Read("select * from companies").ToList();
            Assert.Single(read);
            Assert.Equal("vb-software", read.First()["name"]);
        }

        [Fact]
        public void TestEmployeesTable()
        {
            var read = Connection.Read("select * from employees where company_id = (select id from companies limit 1)").ToList();
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
