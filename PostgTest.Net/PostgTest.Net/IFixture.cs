using Npgsql;

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