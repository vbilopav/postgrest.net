using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;
using Microsoft.Extensions.Options;

namespace PostgRest.net
{
    public interface IResponse
    {
        IResponse SetStatusCode(int? statusCode);
        IResponse SetContentType(string contentType);
        IResponse SetDefaultValue(string defaultValue);
    }

    public interface IContentService : IResponse
    {
        Task<ContentResult> GetContentAsync(string command, Action<NpgsqlParameterCollection> parameters);
        Task<ContentResult> GetContentAsync(string command, Func<NpgsqlParameterCollection, Task> parameters);
        Task<ContentResult> GetContentAsync(string command);
    }

    public class ContentService : IContentService
    {
        private readonly IDataService data;
        private readonly ILogger<ContentService> logger;

        private string defaultValue;
        private string contentType;
        private int? statusCode;

        public ContentService(IDataService data, ILogger<ContentService> logger, IOptions<PostgRestConfig> options)
        {
            this.data = data;
            this.logger = logger;
            this.defaultValue = options.Value.JsonDefaultValue;
            this.contentType = options.Value.JsonContentType;
            this.statusCode = 200;
        }

        public IResponse SetStatusCode(int? statusCode)
        {
            this.statusCode = statusCode;
            return this;
        }

        public IResponse SetContentType(string contentType)
        {
            this.contentType = contentType;
            return this;
        }

        public IResponse SetDefaultValue(string defaultValue)
        {
            this.defaultValue = defaultValue;
            return this;
        }

        public async Task<ContentResult> GetContentAsync(string command, Action<NpgsqlParameterCollection> parameters) =>
            await TryGetContentAsync(async () => await data.GetStringAsync(command, parameters) ?? defaultValue);

        public async Task<ContentResult> GetContentAsync(string command, Func<NpgsqlParameterCollection, Task> parameters) =>
            await TryGetContentAsync(async () => await data.GetStringAsync(command, parameters) ?? defaultValue);

        public async Task<ContentResult> GetContentAsync(string command) =>
            await TryGetContentAsync(async () => await data.GetStringAsync(command) ?? defaultValue);

        private async Task<ContentResult> TryGetContentAsync(Func<Task<string>> func)
        {
            try
            {
                return new ContentResult
                {
                    StatusCode = statusCode,
                    Content = await func() ?? defaultValue,
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
            logger.LogError(e, LoggingService.FormatPostgresExceptionMessage(e, data));

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
