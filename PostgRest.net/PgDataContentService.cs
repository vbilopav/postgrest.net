using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;

namespace PostgRest.net
{
    public interface IPgDataContentService
    {
        Task<ContentResult> GetContentAsync(string command, Action<NpgsqlParameterCollection> parameters = null);
        Task<ContentResult> GetContentAsync(string command, Func<NpgsqlParameterCollection, Task> parameters = null);
    }

    public class PgDataContentService : IPgDataContentService
    {
        private readonly IPgDataService data;
        private readonly ILogger<PgDataContentService> logger;
        private const string DefaultValue = "{}";

        public PgDataContentService(IPgDataService data, ILogger<PgDataContentService> logger)
        {
            this.data = data;
            this.logger = logger;
        }

        public async Task<ContentResult> GetContentAsync(string command, Action<NpgsqlParameterCollection> parameters = null)
        {
            try
            {
                return new ContentResult
                {
                    StatusCode = 200,
                    Content = await data.GetString(command, parameters) ?? DefaultValue,
                    ContentType = "application/json"
                };
            }
            catch (PostgresException e)
            {
                logger.LogError(e, FormatPostgresExceptionMessage(e));
                return BadRequestContent(e);
            }
        }

        public async Task<ContentResult> GetContentAsync(string command, Func<NpgsqlParameterCollection, Task> parameters = null)
        {
            try
            {
                return new ContentResult
                {
                    StatusCode = 200,
                    Content = await data.GetString(command, parameters) ?? DefaultValue,
                    ContentType = "application/json"
                };
            }
            catch (PostgresException e)
            {
                logger.LogError(e, FormatPostgresExceptionMessage(e));
                return BadRequestContent(e);
            }
        }

        private static ContentResult BadRequestContent(PostgresException e) => new ContentResult
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
            ContentType = "application/json"
        };

        private static string FormatPostgresExceptionMessage(PostgresException e) => $"{e.Severity}\n" +
            $"Message: {e.Message}\n" +
            $"Detail: {e.Detail}\n" +
            $"Line: {e.Line}\n" +
            $"InternalPosition: {e.InternalPosition}\n" +
            $"Position: {e.Position}\n" +
            $"SqlState: {e.SqlState}\n" +
            $"Statement: {e.Statement}\n" +
            $"ColumnName: {e.ColumnName}\n" +
            $"ConstraintName: {e.ConstraintName}\n" +
            $"TableName: {e.TableName}\n" +
            $"InternalQuery: {e.InternalQuery}\n" +
            $"Where: {e.Where}\n" +
            $"Hint: {e.Hint}\n\n";
    }
}
