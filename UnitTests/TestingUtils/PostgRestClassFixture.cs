using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    [Collection("testing database")]
    public abstract class PostgRestClassFixture<TConfigureServices, TLifeCycle> :
        IClassFixture<AspNetCoreFixture<TConfigureServices, TLifeCycle>>
        where TConfigureServices : IConfigureServices, new()
        where TLifeCycle : ILifeCycle, new()
    {
        protected readonly ITestOutputHelper output;

        public PostgRestClassFixture(
            ITestOutputHelper output,
            AspNetCoreFixture<TConfigureServices, TLifeCycle> fixture)
        {
            this.output = output;
            fixture.Initialize(this.output);
        }
    }

    [Collection("testing database")]
    public abstract class PostgRestClassFixture<TConfigureServices> :
        IClassFixture<AspNetCoreFixture<TConfigureServices>>
        where TConfigureServices : IConfigureServices, new()
    {
        protected readonly ITestOutputHelper output;

        public PostgRestClassFixture(
            ITestOutputHelper output,
            AspNetCoreFixture<TConfigureServices> fixture)
        {
            this.output = output;
            fixture.Initialize(this.output);
        }
    }
}
