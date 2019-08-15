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
        private readonly IPgLoggingService loggingService;

        public PgDataService(NpgsqlConnection connection, IPgLoggingService loggingService)
        {
            this.connection = connection;
            this.loggingService = loggingService;
        }

        public async Task<string> GetStringAsync(string command, Action<NpgsqlParameterCollection> parameters)
        {
            connection.Notice += loggingService.GetPgNoticeEventHandler(command);
            await EnsureConnectionIsOpen();
            using (var cmd = new NpgsqlCommand(command, connection))
            {
                parameters?.Invoke(cmd.Parameters);
                return await GetStringContentFromCommand(cmd);
            }
        }

        public async Task<string> GetStringAsync(string command, Func<NpgsqlParameterCollection, Task> parameters)
        {
            connection.Notice += loggingService.GetPgNoticeEventHandler(command);
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
            connection.Notice += loggingService.GetPgNoticeEventHandler(command);
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
    }
}
