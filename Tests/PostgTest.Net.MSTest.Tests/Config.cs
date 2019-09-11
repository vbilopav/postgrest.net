using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PostgTest.Net.MSTest.Tests
{
    public class Config : PostgTestConfig
    {
        public override int Port => 5433;
    }

    [TestClass]
    public class UnitTest1
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext ctx) => PostgreSqlFixture.AssemblyInitialize(ctx);


        [AssemblyCleanup]
        public static void AssemblyCleanup() => PostgreSqlFixture.AssemblyCleanup();
    }
}
