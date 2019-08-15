﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace PostgRest.net
{
    public interface IPgLoggingService
    {
        NoticeEventHandler GetPgNoticeEventHandler(string command);
    }

    public class PgLoggingService : IPgLoggingService
    {
        private readonly ILoggerFactory loggerFactory;
        private static readonly IEnumerable<string> InfoLevels = new[] { "INFO", "NOTICE", "LOG" };
        private static readonly IEnumerable<string> ErrorLevels = new[] { "ERROR", "PANIC" };

        public PgLoggingService(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        public NoticeEventHandler GetPgNoticeEventHandler(string command)
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
    }
}
