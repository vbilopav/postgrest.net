using System.Linq;
using Xunit;

namespace PostgTest.Net.XUnit.Tests
{
    [Collection("PostgreSqlTestDatabase")]
    public class PostgreSqlTransactionIsolationTests : PostgreSqlUnitTest
    {
        private static int? _sharedTxid;

        public PostgreSqlTransactionIsolationTests(PostgreSqlFixture fixture) : base(fixture)
        {

        }

        [Fact]
        public void TestTransaction1()
        {
            var txid1 = Read("select txid_current()").ToList().First()["txid_current"] as int?;
            if (_sharedTxid == null)
            {
                _sharedTxid = txid1;
            }
            else
            {
                Assert.NotEqual(_sharedTxid, txid1);
            }
            var txid2 = Read("select txid_current()").ToList().First()["txid_current"] as int?;
            Assert.Equal(txid2, txid1);
        }

        [Fact]
        public void TestTransaction2()
        {
            var txid1 = Read("select txid_current()").ToList().First()["txid_current"] as int?;
            if (_sharedTxid == null)
            {
                _sharedTxid = txid1;
            }
            else
            {
                Assert.NotEqual(_sharedTxid, txid1);
            }
            var txid2 = Read("select txid_current()").ToList().First()["txid_current"] as int?;
            Assert.Equal(txid2, txid1);
        }
    }
}
