using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace PostgRest.net
{
    public interface ILoggingService
    {
        NoticeEventHandler CreateNoticeEventHandler(string command);
    }

    public class LoggingService : ILoggingService
    {
        private readonly ILoggerFactory loggerFactory;
        private static readonly IEnumerable<string> InfoLevels = new[] { "INFO", "NOTICE", "LOG" };
        private static readonly IEnumerable<string> ErrorLevels = new[] { "ERROR", "PANIC" };

        public LoggingService(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        public NoticeEventHandler CreateNoticeEventHandler(string command)
        {
            var logger = loggerFactory.CreateLogger(command);
            return (sender, args) =>
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

        public static string FormatPostgresExceptionMessage(PostgresException e, IDataService data) => $"{e.Severity}\n" +
            $"Message: {e.Message}\n" +
            $"Detail: {e.Detail}\n" +
            $"Line: {e.Line}\n" +
            $"InternalPosition: {e.InternalPosition}\n" +
            $"Position: {e.Position}\n" +
            $"SqlState: {e.SqlState}\n" +
            $"Statement: {e.Statement}\n" +
            FormatParametersInfo(data) +
            $"ColumnName: {e.ColumnName}\n" +
            $"ConstraintName: {e.ConstraintName}\n" +
            $"TableName: {e.TableName}\n" +
            $"InternalQuery: {e.InternalQuery}\n" +
            $"Where: {e.Where}\n" +
            $"Hint: {e.Hint}\n\n";

        private static string FormatParametersInfo(IDataService data)
        {
            var info = data?.GetParameterInfo();
            if (info == null || info.Count == 0)
            {
                return string.Empty;
            }
            return string.Concat("Parameters: ", string.Join(", ", info.Select(p => $"@{p.Key}={p.Value}")), "\n");
        }
    }
}
