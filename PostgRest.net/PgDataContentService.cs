using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;

namespace PostgRest.net
{
    public interface IPgDataContentService
    {
        Task<ContentResult> GetContentAsync(string command, Action<NpgsqlParameterCollection> parameters);
        Task<ContentResult> GetContentAsync(string command, Func<NpgsqlParameterCollection, Task> parameters);
        Task<ContentResult> GetContentAsync(string command);
        void SetContentParameters(int? statusCode, string contentType, string emptyContentValue);
    }

    public class PgDataContentService : IPgDataContentService
    {
        private readonly IPgDataService data;
        private readonly ILogger<PgDataContentService> logger;

        private string emptyContentValue = "{}";
        private string contentType = "application/json";
        private int? statusCode = 200;

        public PgDataContentService(IPgDataService data, ILogger<PgDataContentService> logger)
        {
            this.data = data;
            this.logger = logger;
        }

        public void SetContentParameters(int? statusCode, string contentType, string emptyContentValue)
        {
            this.statusCode = statusCode;
            this.contentType = contentType;
            this.emptyContentValue = emptyContentValue;
        }

        public async Task<ContentResult> GetContentAsync(string command, Action<NpgsqlParameterCollection> parameters) =>
            await TryGetContentAsync(async () => await data.GetStringAsync(command, parameters) ?? emptyContentValue);

        public async Task<ContentResult> GetContentAsync(string command, Func<NpgsqlParameterCollection, Task> parameters) =>
            await TryGetContentAsync(async () => await data.GetStringAsync(command, parameters) ?? emptyContentValue);

        public async Task<ContentResult> GetContentAsync(string command) =>
            await TryGetContentAsync(async () => await data.GetStringAsync(command) ?? emptyContentValue);

        private async Task<ContentResult> TryGetContentAsync(Func<Task<string>> func)
        {
            try
            {
                return new ContentResult
                {
                    StatusCode = statusCode,
                    Content = await func() ?? emptyContentValue,
                    ContentType = contentType
                };
            }
            catch (PostgresException e)
            {
                return GetExceptionContent(e);
            }
        }

        private ContentResult GetExceptionContent(PostgresException e)
        {
            logger.LogError(e, PgLoggingService.FormatPostgresExceptionMessage(e));

            // insufficient_privilege, see: https://www.postgresql.org/docs/11/errcodes-appendix.html
            if (e.SqlState == "42501")
            {
                return UnathorizedContent();
            } else
            {
                return BadRequestContent(e);
            }
        }

        private ContentResult UnathorizedContent() => new ContentResult
        {
            StatusCode = 401,
            Content = JsonConvert.SerializeObject(new
            {
                messeage = "Unathorized",
                error = true
            }),
            ContentType = contentType
        };

        private ContentResult BadRequestContent(PostgresException e) => new ContentResult
        {
            StatusCode = 400,
            Content = JsonConvert.SerializeObject(new
            {
                messeage = e.MessageText,
                details = e.Detail,
                table = e.TableName,
                column = e.ColumnName,
                constraint = e.ConstraintName,
                error = true
            }),
            ContentType = contentType
        };
    }
}
