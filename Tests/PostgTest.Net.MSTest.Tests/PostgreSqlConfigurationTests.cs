using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PostgTest.Net.MSTest.Tests
{
    [TestClass]
    public class PostgreSqlConfigurationTests : PostgreSqlUnitTest
    {
        [TestMethod]
        public void TestDatabaseName()
        {
            var read = Read("select current_database()").ToList();
            Assert.AreEqual(1, read.Count);
            Assert.AreEqual(Config.TestDatabase, read.First()["current_database"]);
        }

        [TestMethod]
        public void TestSessionUserName()
        {
            var read = Read("select session_user").ToList();
            Assert.AreEqual(1, read.Count);
            Assert.AreEqual(Config.TestUser, read.First()["session_user"]);
        }

        [TestMethod]
        public void TestCurrentUserName()
        {
            var read = Read("select current_user").ToList();
            Assert.AreEqual(1, read.Count);
            Assert.AreEqual(Config.TestUser, read.First()["current_user"]);
        }

        [TestMethod]
        public void DumpBackendPid()
        {
            var read = Read("select pg_backend_pid()").ToList();
            Assert.AreEqual(1, read.Count);
            Debug.WriteLine($"pg_backend_pid = {read.First()["pg_backend_pid"]}");
        }
    }
}
