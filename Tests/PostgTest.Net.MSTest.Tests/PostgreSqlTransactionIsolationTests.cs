using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace PostgTest.Net.MSTest.Tests
{
    [TestClass]
    public class PostgreSqlTransactionIsolationTests : PostgreSqlUnitTest
    {
        private static int? _sharedTxid;

        [TestMethod]
        public void TestTransaction1()
        {
            var txid1 = Read("select txid_current()").ToList().First()["txid_current"] as int?;
            if (_sharedTxid == null)
            {
                _sharedTxid = txid1;
            }
            else
            {
                Assert.AreNotEqual(_sharedTxid, txid1);
            }
            var txid2 = Read("select txid_current()").ToList().First()["txid_current"] as int?;
            Assert.AreEqual(txid2, txid1);
        }

        [TestMethod]
        public void TestTransaction2()
        {
            var txid1 = Read("select txid_current()").ToList().First()["txid_current"] as int?;
            if (_sharedTxid == null)
            {
                _sharedTxid = txid1;
            }
            else
            {
                Assert.AreNotEqual(_sharedTxid, txid1);
            }
            var txid2 = Read("select txid_current()").ToList().First()["txid_current"] as int?;
            Assert.AreEqual(txid2, txid1);
        }
    }
}
