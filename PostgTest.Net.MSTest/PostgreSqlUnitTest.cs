using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PostgTest.Net.MSTest
{
    [TestClass]
    public abstract class PostgreSqlUnitTest : Net.PostgreSqlUnitTest
    {
        [TestInitialize]
        public void Setup()
        {
            Initialize(PostgreSqlFixture.Fixture);
        }

        [TestCleanup]
        public void TearDown()
        {
            Dispose();
        }

        protected PostgreSqlUnitTest() : base(null)
        {
        }
    }
}
