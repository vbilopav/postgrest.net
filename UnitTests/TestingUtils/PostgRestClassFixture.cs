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
            AspNetCoreFixture<TConfigureServices, TLifeCycle> fixture, 
            string url = null)
        {
            this.output = output;
            fixture.Initialize(this.output, url);
        }
    }

    [Collection("testing database")]
    public abstract class PostgRestClassFixture<TLifeCycle> :
        IClassFixture<AspNetCoreFixture<TLifeCycle>>
        where TLifeCycle : ILifeCycle, new()
    {
        protected readonly ITestOutputHelper output;

        public PostgRestClassFixture(
            ITestOutputHelper output,
            AspNetCoreFixture<TLifeCycle> fixture, 
            string url = null)
        {
            this.output = output;
            fixture.Initialize(this.output, url);
        }
    }

    /*
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
    */
}
