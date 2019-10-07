using System;
using Norm.Extensions;
using Xunit;

namespace PostgTest.Net.TransactionIsolationTests
{
    /// <summary>
    /// Every test is SAME transaction
    /// </summary>
    [Collection("PostgreSqlTestDatabase")]
    public class TransactionIsolationClassTests : IClassFixture<PostgreSqlTestFixture>
    {
        private static int? _sharedTxid;
        private readonly PostgreSqlTestFixture fixture;

        public TransactionIsolationClassTests(PostgreSqlTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void TestTransaction1()
        {
            var txid1 = fixture.TestConnection.Single<int>("select txid_current()");
            if (_sharedTxid == null)
            {
                _sharedTxid = txid1;
            }
            else
            {
                Assert.Equal(_sharedTxid, Convert.ToInt32(txid1));
            }
            var txid2 = fixture.TestConnection.Single<int>("select txid_current()");
            Assert.Equal(txid2, txid1);
        }

        [Fact]
        public void TestTransaction2()
        {
            var txid1 = fixture.TestConnection.Single<int>("select txid_current()");
            if (_sharedTxid == null)
            {
                _sharedTxid = Convert.ToInt32(txid1);
            }
            else
            {
                Assert.Equal(_sharedTxid, Convert.ToInt32(txid1));
            }
            var txid2 = fixture.TestConnection.Single<int>("select txid_current()");
            Assert.Equal(txid2, txid1);
        }
    }
}
