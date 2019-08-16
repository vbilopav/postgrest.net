using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using static UnitTests.Config;

namespace UnitTests
{
    public class LoggingTests : PostgRestClassFixture<DefaultConfig, LoggingTests.LifeCycle>
    {
        public class LifeCycle : ILifeCycle
        {
            public void BuildUp() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"

            create function rest__get_test_logging_info() returns json as
            $$
            begin
                raise info 'info log';
                return '{}';
            end
            $$ language plpgsql;

            create function rest__get_test_logging_notice() returns json as
            $$
            begin
                raise notice 'notice log';
                return '{}';
            end
            $$ language plpgsql;

            create function rest__get_test_logging_log() returns json as
            $$
            begin
                set client_min_messages to 'log';
                raise log 'log log';
                return '{}';
            end
            $$ language plpgsql;


            create function rest__get_test_logging_warn() returns json as
            $$
            begin
                raise warning 'warn log';
                return '{}';
            end
            $$ language plpgsql;

            create function rest__get_test_logging_debug() returns json as
            $$
            begin
                set client_min_messages to 'debug';
                raise warning 'debug log';
                return '{}';
            end
            $$ language plpgsql;

            create function rest__get_test_logging_error() returns json as
            $$
            begin
                raise exception 'error log';
                return '{}';
            end
            $$ language plpgsql;

            ");

            public void TearDown() => DatabaseFixture.ExecuteCommand(ConnectionType.PostgresTesting, @"

            drop function rest__get_test_logging_info();
            drop function rest__get_test_logging_notice();
            drop function rest__get_test_logging_log();
            drop function rest__get_test_logging_warn();
            drop function rest__get_test_logging_debug();
            drop function rest__get_test_logging_error();

            ");

        }

        public class TestOutputHelperAdapter : ITestOutputHelper
        {
            private Action<string> injected;
            ITestOutputHelper outputHelper;

            public TestOutputHelperAdapter(ITestOutputHelper outputHelper)
            {
                this.outputHelper = outputHelper;
            }

            public void Inject(Action<string> injected)
            {
                this.injected = injected;
            }

            public void WriteLine(string message)
            {
                injected?.Invoke(message);
                outputHelper.WriteLine(message);
            }

            public void WriteLine(string format, params object[] args) { }
        }

        public LoggingTests(
            ITestOutputHelper output,
            AspNetCoreFixture<DefaultConfig, LifeCycle> fixture) : base(new TestOutputHelperAdapter(output), fixture) {}

        [Fact]
        public async Task TestLogWriteFromPostgresFunction()
        {
            bool infoFound = false;
            bool noticeFound = false;
            bool logFound = false;
            bool warnFound = false;
            bool debugFound = false;
            bool errorFound = false;
            (output as TestOutputHelperAdapter).Inject(message =>
            {
                if (!infoFound)
                {
                    infoFound = string.Equals(message, "[Information] select rest__get_test_logging_info() [0] info log");
                }
                if (!noticeFound)
                {
                    noticeFound = string.Equals(message, "[Information] select rest__get_test_logging_notice() [0] notice log");
                }
                if (!logFound)
                {
                    logFound = string.Equals(message, "[Information] select rest__get_test_logging_log() [0] log log");
                }
                if (!warnFound)
                {
                    warnFound = string.Equals(message, "[Warning] select rest__get_test_logging_warn() [0] warn log");
                }
                if (!debugFound)
                {
                    debugFound = string.Equals(message, "[Warning] select rest__get_test_logging_debug() [0] debug log");
                }
                if (!errorFound)
                {
                    errorFound = string.Equals(message, "[Error] PostgRest.net.ContentService [0] ERROR\nMessage: P0001: error log\nDetail: \nLine: 3778\nInternalPosition: 0\nPosition: 0\nSqlState: P0001\nStatement: select rest__get_test_logging_error()\nColumnName: \nConstraintName: \nTableName: \nInternalQuery: \nWhere: PL/pgSQL function rest__get_test_logging_error() line 3 at RAISE\nHint: \n\n");
                }
            });

            using (var client = new HttpClient())
            {
                await RestClient.GetAsync<object>("https://localhost:5001/api/test-logging-info", client);
                await RestClient.GetAsync<object>("https://localhost:5001/api/test-logging-notice", client);
                await RestClient.GetAsync<object>("https://localhost:5001/api/test-logging-log", client);
                await RestClient.GetAsync<object>("https://localhost:5001/api/test-logging-warn", client);
                await RestClient.GetAsync<object>("https://localhost:5001/api/test-logging-debug", client);
                await RestClient.GetAsync<object>("https://localhost:5001/api/test-logging-error", client);
            }

            Assert.True(infoFound);
            Assert.True(noticeFound);
            Assert.True(logFound);
            Assert.True(warnFound);
            Assert.True(debugFound);
            Assert.True(errorFound);
        }
    }
}
