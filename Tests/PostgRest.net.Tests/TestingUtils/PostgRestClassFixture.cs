using Xunit;
using Xunit.Abstractions;

namespace PostgRest.net.Tests.TestingUtils
{
    [Collection("testing database")]
    public abstract class PostgRestClassFixture<TConfigureServices, TLifeCycle> :
        IClassFixture<AspNetCoreFixture<TConfigureServices, TLifeCycle>>
        where TConfigureServices : IConfigureServices, new()
        where TLifeCycle : ILifeCycle, new()
    {
        protected readonly ITestOutputHelper Output;

        protected PostgRestClassFixture(
            ITestOutputHelper output,
            AspNetCoreFixture<TConfigureServices, TLifeCycle> fixture, 
            string url = null)
        {
            this.Output = output;
            fixture.Initialize(this.Output, url);
        }
    }

    [Collection("testing database")]
    public abstract class PostgRestClassFixture<TLifeCycle> :
        IClassFixture<AspNetCoreFixture<TLifeCycle>>
        where TLifeCycle : ILifeCycle, new()
    {
        protected readonly ITestOutputHelper Output;

        protected PostgRestClassFixture(
            ITestOutputHelper output,
            AspNetCoreFixture<TLifeCycle> fixture, 
            string url = null)
        {
            this.Output = output;
            fixture.Initialize(this.Output, url);
        }
    }
}
