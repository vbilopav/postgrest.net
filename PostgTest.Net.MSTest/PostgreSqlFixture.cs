using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PostgTest.Net.MSTest
{
    [TestClass]
    public class PostgreSqlFixture
    {
        public static IPostgreSqlFixture Fixture;

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext ctx)
        {
            Fixture = new Net.PostgreSqlFixture();
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            Fixture.Dispose();
        }
    }
}
