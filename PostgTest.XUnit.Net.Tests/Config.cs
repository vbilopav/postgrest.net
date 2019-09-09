using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PostgTest.XUnit.Net.Tests
{
    public class Config : PostgreSqlTestConfig
    {
        public override int Port => 5433;
    }

    [CollectionDefinition("PostgreSqlTestDatabase")]
    public class PostgreSqlFixtureCollection : ICollectionFixture<PostgreSqlFixture> { }
}
