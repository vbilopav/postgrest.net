using System;
using PostgTest.XUnit.Net;
using Xunit;
using Xunit.Abstractions;

namespace ValuesControllerTests
{
    public class MyConfig : PostgreSqlTestConfig
    {
        public override string TestUser { get; set; } = "my_user";
    }

    public class UnitTest1
    {
        private readonly ITestOutputHelper output;

        public UnitTest1(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Test1()
        {
            var config = Config<MyConfig>.Value;
            output.WriteLine("CreateTestUser = {0}", config.CreateTestUser);
        }
    }
}
