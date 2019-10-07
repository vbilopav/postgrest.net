using System;
using Microsoft.Extensions.Configuration;

namespace PostgTest.Net.Configuration
{
    public static class Config
    {
        private const string ConfigSection = "PostgTest";
        private const string ConfigTypeKey = "PostgTestConfigType";

        static Config()
        {
            PostgTestConfig instance = null;

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, false)
                .AddJsonFile("appsettings.test.json", true, false)
                .AddJsonFile("testsettings.json", true, false)
                .AddJsonFile("settings.json", true, false)
                .Build();

            var configTypeName = config[ConfigTypeKey];
            if (configTypeName != null)
            {
                var type = Type.GetType(configTypeName);
                if (type != null)
                {
                    instance = Activator.CreateInstance(type) as PostgTestConfig;
                }
            }
            if (instance == null)
            {
                instance = new PostgTestConfig();
            }
            config?.Bind(ConfigSection, instance);
            Value = instance;
        }

        public static PostgTestConfig Value { get; }
    }
}
