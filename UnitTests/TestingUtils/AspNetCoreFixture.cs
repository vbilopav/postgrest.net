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

    public class AspNetCoreFixture<T> : IDisposable where T : IConfigureServices, new()
    {
        private IWebHost host;

        public AspNetCoreFixture()
        {
            host = null;
        }

        public void Initialize(ITestOutputHelper output)
        {
            host = WebHost.CreateDefaultBuilder()
                .ConfigureLogging(f => f.AddProvider(new XUnitLoggerProvider(output)))
                .UseStartup<Startup<T>>()
                .Build();
            host.Start();
        }

        public void Dispose()
        {
            if (host != null)
            {
                host.Dispose();
            }
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
