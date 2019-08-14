﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PostgRest.net;

namespace SampleRestApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {        /*
            services.AddSingleton<NpgsqlConnection, NpgsqlConnection>(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                var connStr = config.GetPgCloudConnectionString("SampleConnection") ?? "SampleConnection";
                return new NpgsqlConnection(connStr);
            });
               */
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddPostgRest(services, new PostgRestOptions
                {
                    //Connection = "PostgreSqlConnection
                    Connection = "SampleConnection"
                    //ApplyFilters = (filters, route, func) =>
                    //{
                    //    add filters ... authorization, roles, cors, etc...
                    //}
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}