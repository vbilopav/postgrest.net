using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace PostgTest.XUnit.Net
{
    public static class Config<T> where T : IPostgreSqlTestConfig, new()
    {
        static Config()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, false)
                .AddJsonFile("appsettings.test.json", true, false)
                .Build();

            Value = new T();
            config?.Bind("PostgRest", Value);
        }

        public static T Value { get; }
    }
}
