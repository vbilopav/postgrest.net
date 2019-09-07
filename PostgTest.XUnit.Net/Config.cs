using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace PostgTest.XUnit.Net
{
    public static class Config<T> where T : IPostgreSqlTestConfig, new()
    {
        static Config()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, false)
                .AddJsonFile("appsettings.test.json", true, false)
                .AddJsonFile("testsettings.json", true, false)
                .AddJsonFile("settings.json", true, false)
                .Build();

            Value = new T();
            config?.Bind("PostgTest", Value);
        }

        public static T Value { get; }
    }

    public static class Config
    {
        static Config()
        {
            Value = Config<PostgreSqlTestConfig>.Value;
        }

        public static PostgreSqlTestConfig Value { get; }
    }
}
