using System.Linq;
using Npgsql;
using Norm.Extensions;
using Xunit;

namespace PostgTest.Net.DbMigrationCodeClassTests
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
            var read = Connection.Read("select * from companies").SelectDictionaries().ToList();

            Assert.Single(read);
            Assert.Equal("vb-software", read.First()["name"]);
        }

        [Fact]
        public void TestEmployeesTable()
        {
            var list = Connection
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
