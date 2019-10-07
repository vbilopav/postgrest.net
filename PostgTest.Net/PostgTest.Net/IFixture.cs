using Npgsql;
using PostgTest.Net.Configuration;
using PostgTest.Net.Migrations;

namespace PostgTest.Net
{
    public interface IFixture
    {
        NpgsqlConnection TestConnection { get; }
        NpgsqlConnection DefaultConnection { get; }
        IPostgTestConfig Configuration { get; }
        MigrationBase Migration { get; }
    }
}