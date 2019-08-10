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
        Task<string> GetString(string command, Action<NpgsqlParameterCollection> parameters = null);
        Task<string> GetString(string command, Func<NpgsqlParameterCollection, Task> parameters = null);
    }

    public class PgDataService : IPgDataService
    {
        private readonly NpgsqlConnection _connection;
        private readonly ILoggerFactory _loggerFactory;
        private static readonly IEnumerable<string> InfoLevels = new[] { "INFO", "NOTICE", "LOG" };
        private static readonly IEnumerable<string> ErrorLevels = new[] { "ERROR", "PANIC" };

        public PgDataService(NpgsqlConnection connection, ILoggerFactory loggerFactory)
        {
            _connection = connection;
            _loggerFactory = loggerFactory;
        }

        public async Task<string> GetString(string command, Action<NpgsqlParameterCollection> parameters = null)
        {
            HandleLogging(command);
            await _connection.OpenAsync();
            using (var cmd = new NpgsqlCommand(command, _connection))
            {
                parameters?.Invoke(cmd.Parameters);
                return await GetStringContentFromCommand(cmd);
            }
        }

        public async Task<string> GetString(string command, Func<NpgsqlParameterCollection, Task> parameters = null)
        {
            HandleLogging(command);
            await _connection.OpenAsync();
            using (var cmd = new NpgsqlCommand(command, _connection))
            {
                if (parameters != null)
                {
                    await parameters?.Invoke(cmd.Parameters);
                }
                return await GetStringContentFromCommand(cmd);
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

        private void HandleLogging(string command)
        {
            var logger = _loggerFactory.CreateLogger(command);
            _connection.Notice += (sender, args) =>
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
