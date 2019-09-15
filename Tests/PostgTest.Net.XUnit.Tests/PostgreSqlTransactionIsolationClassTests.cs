using System;
using Xunit;
using PostgExecute.Net;

namespace PostgTest.Net.XUnit.Tests
{
    /// <summary>
    /// Every test is SAME transaction
    /// </summary>
    [Collection("PostgreSqlTestDatabase")]
    public class PostgreSqlTransactionIsolationClassTests : IClassFixture<PostgreSqlTestFixture>
    {
        private static int? _sharedTxid;
        private readonly PostgreSqlTestFixture fixture;

        public PostgreSqlTransactionIsolationClassTests(PostgreSqlTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void TestTransaction1()
        {
            var txid1 = Convert.ToInt32(fixture.Connection.Single("select txid_current()")["txid_current"]);
            if (_sharedTxid == null)
            {
                _sharedTxid = txid1;
            }
            else
            {
                Assert.Equal(_sharedTxid, Convert.ToInt32(txid1));
            }
            var txid2 = Convert.ToInt32(fixture.Connection.Single("select txid_current()")["txid_current"]);
            Assert.Equal(txid2, txid1);
        }

        [Fact]
        public void TestTransaction2()
        {
            var txid1 = Convert.ToInt32(fixture.Connection.Single("select txid_current()")["txid_current"]);
            if (_sharedTxid == null)
            {
                _sharedTxid = Convert.ToInt32(txid1);
            }
            else
            {
                Assert.Equal(_sharedTxid, Convert.ToInt32(txid1));
            }
            var txid2 = Convert.ToInt32(fixture.Connection.Single("select txid_current()")["txid_current"]);
            Assert.Equal(txid2, txid1);
        }
    }
}
