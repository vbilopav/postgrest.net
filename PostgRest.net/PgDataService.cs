using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace PostgRest.net
{
    public interface IPgDataService
    {
        Task<string> GetStringAsync(string command, Action<NpgsqlParameterCollection> parameters);
        Task<string> GetStringAsync(string command, Func<NpgsqlParameterCollection, Task> parameters);
        Task<string> GetStringAsync(string command);
    }

    public class PgDataService : IPgDataService
    {
        private readonly NpgsqlConnection connection;
        private readonly ILoggerFactory loggerFactory;
        private static readonly IEnumerable<string> InfoLevels = new[] { "INFO", "NOTICE", "LOG" };
        private static readonly IEnumerable<string> ErrorLevels = new[] { "ERROR", "PANIC" };

        public PgDataService(NpgsqlConnection connection, ILoggerFactory loggerFactory)
        {
            this.connection = connection;
            this.loggerFactory = loggerFactory;
        }

        public async Task<string> GetStringAsync(string command, Action<NpgsqlParameterCollection> parameters)
        {
            AddLoggingToNoticeHandler(command);
            await EnsureConnectionIsOpen();
            using (var cmd = new NpgsqlCommand(command, connection))
            {
                parameters?.Invoke(cmd.Parameters);
                return await GetStringContentFromCommand(cmd);
            }
        }

        public async Task<string> GetStringAsync(string command, Func<NpgsqlParameterCollection, Task> parameters)
        {
            AddLoggingToNoticeHandler(command);
            await EnsureConnectionIsOpen();
            using (var cmd = new NpgsqlCommand(command, connection))
            {
                if (parameters != null)
                {
                    await parameters?.Invoke(cmd.Parameters);
                }
                return await GetStringContentFromCommand(cmd);
            }
        }

        public async Task<string> GetStringAsync(string command)
        {
            AddLoggingToNoticeHandler(command);
            await EnsureConnectionIsOpen();
            using (var cmd = new NpgsqlCommand(command, connection))
            {
                return await GetStringContentFromCommand(cmd);
            }
        }

        private async Task EnsureConnectionIsOpen()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }
        }

        private static async Task<string> GetStringContentFromCommand(NpgsqlCommand cmd)
        {
            using (var reader = cmd.ExecuteReader())
            {
                if (!await reader.ReadAsync())
                {
                    return null;
                }
                return reader.GetFieldType(0) == DBNull.Value.GetType() ? null : reader.GetString(0);
            }
        }

        private void AddLoggingToNoticeHandler(string command)
        {
            var logger = loggerFactory.CreateLogger(command);
            connection.Notice += (sender, args) =>
            {
                var severity = args.Notice.Severity;
                if (InfoLevels.Contains(severity))
                {
                    logger.LogInformation(args.Notice.MessageText);
                }
                else if (severity == "WARNING")
                {
                    logger.LogWarning(args.Notice.MessageText);
                }
                else if (severity.StartsWith("DEBUG"))
                {
                    logger.LogDebug(args.Notice.MessageText);
                }
                else if (ErrorLevels.Contains(severity))
                {
                    logger.LogError(args.Notice.MessageText);
                }
                else
                {
                    logger.LogTrace(args.Notice.MessageText);
                }
            };
        }
    }
}
