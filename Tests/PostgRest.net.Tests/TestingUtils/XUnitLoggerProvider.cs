using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace PostgRest.net.Tests.TestingUtils
{
    public class XUnitLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper testOutputHelper;

        public XUnitLoggerProvider(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        public ILogger CreateLogger(string categoryName)
            => new XUnitLogger(testOutputHelper, categoryName);

        public void Dispose()
        { }
    }

    public class XUnitLogger : ILogger
    {
        private readonly ITestOutputHelper testOutputHelper;
        private readonly string categoryName;

        public XUnitLogger(ITestOutputHelper testOutputHelper, string categoryName)
        {
            this.testOutputHelper = testOutputHelper;
            this.categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
            => NoopDisposable.Instance;

        public bool IsEnabled(LogLevel logLevel)
            => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            testOutputHelper.WriteLine($"[{logLevel}] {categoryName} [{eventId}] {formatter(state, exception)}");
            if (exception != null)
            {
                testOutputHelper.WriteLine(exception.ToString());
            }
        }

        private class NoopDisposable : IDisposable
        {
            public static readonly NoopDisposable Instance = new NoopDisposable();
            public void Dispose() { }
        }
    }
}
