using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit.Abstractions;


namespace UnitTests
{
    public interface IConfigureServices
    {
        void ConfigureServices(IServiceCollection services);
    }

    public interface ILifeCycle
    {
        void BuildUp();
        void TearDown();
    }

    public class AspNetCoreFixture<TLifeCycle> : AspNetCoreFixture<DefaultConfigServices, TLifeCycle>
        where TLifeCycle : ILifeCycle, new()
    {
        public AspNetCoreFixture() : base() {}
    }

    public class AspNetCoreFixture<TConfigureServices, TLifeCycle> : IDisposable
        where TConfigureServices : IConfigureServices, new()
        where TLifeCycle : ILifeCycle, new()
    {
        private IWebHost host;
        private readonly ILifeCycle lifeCycle;

        public AspNetCoreFixture()
        {
            host = null;
            lifeCycle = new TLifeCycle();
            lifeCycle.BuildUp();
        }

        public void Initialize(ITestOutputHelper output, string url)
        {
            if (host != null)
            {
                return;
            }
            var builder = WebHost.CreateDefaultBuilder()
                .ConfigureLogging(f => f.AddProvider(new XUnitLoggerProvider(output)))
                .UseStartup<Startup<TConfigureServices>>();
            if (url != null)
            {
                builder = builder.UseUrls(url);
            }
            host = builder.Build();
            host.Start();
        }

        public void Dispose()
        {
            if (host != null)
            {
                host.Dispose();
            }
            lifeCycle.TearDown();
        }
    }

    public class Startup<T> where T : IConfigureServices, new()
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var configure = new T();
            configure.ConfigureServices(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
