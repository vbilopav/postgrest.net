using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    [Collection("testing database")]
    public abstract class PostgRestClassFixture<T> : IClassFixture<AspNetCoreFixture<T>>
        where T : IConfigureServices, new()
    {
        protected readonly ITestOutputHelper output;

        public PostgRestClassFixture(ITestOutputHelper output, AspNetCoreFixture<T> fixture)
        {
            this.output = output;
            fixture.Initialize(this.output);
        }
    }
}
