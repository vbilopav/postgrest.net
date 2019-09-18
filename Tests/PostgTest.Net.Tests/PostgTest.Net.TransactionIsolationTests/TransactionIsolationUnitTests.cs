using System;
using PostgExecute.Net;
using Xunit;

namespace PostgTest.Net.TransactionIsolationTests
{
    /// <summary>
    /// Every test is DIFFERENT transaction
    /// </summary>
    [Collection("PostgreSqlTestDatabase")]
    public class TransactionIsolationUnitTests : PostgreSqlTestFixture
    {
        private static int? _sharedTxid;

        public TransactionIsolationUnitTests(PostgreSqlDatabaseFixture fixture) : base(fixture)
        {
        }


        [Fact]
        public void TestTransaction1()
        {
            var txid1 = Convert.ToInt32(TestConnection.Single("select txid_current()")["txid_current"]);
            if (_sharedTxid == null)
            {
                _sharedTxid = txid1;
            }
            else
            {
                Assert.NotEqual(_sharedTxid, txid1);
            }

            var txid2 = Convert.ToInt32(TestConnection.Single("select txid_current()")["txid_current"]);
            Assert.Equal(txid2, txid1);
        }

        [Fact]
        public void TestTransaction2()
        {
            var txid1 = Convert.ToInt32(TestConnection.Single("select txid_current()")["txid_current"]);
            if (_sharedTxid == null)
            {
                _sharedTxid = txid1;
            }
            else
            {
                Assert.NotEqual(_sharedTxid, txid1);
            }
            var txid2 = Convert.ToInt32(TestConnection.Single("select txid_current()")["txid_current"]);
            Assert.Equal(txid2, txid1);
        }
    }
}
