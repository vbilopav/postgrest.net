using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PostgTest.Net.MSTest
{
    [TestClass]
    public class PostgreSqlFixture
    {
        public static IPostgreSqlFixture Fixture;

        [AssemblyInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
        public static void AssemblyInitialize(TestContext ctx)
#pragma warning restore IDE0060 // Remove unused parameter
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
