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

    public interface IStringContentService : IResponse
    {
        Task<ContentResult> GetContentAsync(string command, Action<NpgsqlParameterCollection> parameters, bool recordset = false);
        Task<ContentResult> GetContentAsync(string command, Func<NpgsqlParameterCollection, Task> parameters, bool recordset = false);
        Task<ContentResult> GetContentAsync(string command, bool recordset = false);
    }

    public class StringContentService : IStringContentService
    {
        private readonly IStringDataService stringData;
        private readonly ILogger<StringContentService> logger;

        private string defaultValue;
        private string contentType;
        private int? statusCode;

        public StringContentService(IStringDataService stringData, ILogger<StringContentService> logger, IOptions<PostgRestConfig> options)
        {
            this.stringData = stringData;
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

        public async Task<ContentResult> GetContentAsync(string command, Action<NpgsqlParameterCollection> parameters, bool recordset = false) =>
            await TryGetContentAsync(async () => await stringData.GetStringAsync(command, parameters, recordset) ?? defaultValue);

        public async Task<ContentResult> GetContentAsync(string command, Func<NpgsqlParameterCollection, Task> parameters, bool recordset = false) =>
            await TryGetContentAsync(async () => await stringData.GetStringAsync(command, parameters, recordset) ?? defaultValue);

        public async Task<ContentResult> GetContentAsync(string command, bool recordset = false) =>
            await TryGetContentAsync(async () => await stringData.GetStringAsync(command, recordset) ?? defaultValue);

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
            logger.LogError(e, LoggingService.FormatPostgresExceptionMessage(e, stringData));

            // state 42501 insufficient_privilege, see: https://www.postgresql.org/docs/11/errcodes-appendix.html
            return e.SqlState == "42501" ? UnauthorizedContent() : BadRequestContent(e);
        }

        private ContentResult UnauthorizedContent() => new ContentResult
        {
            StatusCode = 401,
            Content = JsonConvert.SerializeObject(new
            {
                messeage = "Unauthorized",
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
