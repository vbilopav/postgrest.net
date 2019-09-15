using System;
using Xunit;
using PostgExecute.Net;

namespace PostgTest.Net.XUnit.Tests
{
    [Collection("PostgreSqlTestDatabase")]
    public class PostgreSqlTransactionIsolationUnitTests : PostgreSqlUnitTest
    {
        private static int? _sharedTxid;

        public PostgreSqlTransactionIsolationUnitTests(PostgreSqlDatabaseFixture databaseFixture) : base(databaseFixture)
        {

        }

        [Fact]
        public void TestTransaction1()
        {
            var txid1 = Convert.ToInt32(Connection.Single("select txid_current()")["txid_current"]);
            if (_sharedTxid == null)
            {
                _sharedTxid = txid1;
            }
            else
            {
                Assert.NotEqual(_sharedTxid, txid1);
            }

            var txid2 = Convert.ToInt32(Connection.Single("select txid_current()")["txid_current"]);
            Assert.Equal(txid2, txid1);
        }

        [Fact]
        public void TestTransaction2()
        {
            var txid1 = Convert.ToInt32(Connection.Single("select txid_current()")["txid_current"]);
            if (_sharedTxid == null)
            {
                _sharedTxid = txid1;
            }
            else
            {
                Assert.NotEqual(_sharedTxid, txid1);
            }
            var txid2 = Convert.ToInt32(Connection.Single("select txid_current()")["txid_current"]);
            Assert.Equal(txid2, txid1);
        }
    }
}
