using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace PostgRest.net
{
    public interface IDataService
    {
        Task<string> GetStringAsync(string command, Action<NpgsqlParameterCollection> parameters);
        Task<string> GetStringAsync(string command, Func<NpgsqlParameterCollection, Task> parameters);
        Task<string> GetStringAsync(string command);
        IList<KeyValuePair<string, object>> GetParameterInfo();
    }

    public class DataService : IDataService
    {
        private readonly NpgsqlConnection connection;
        private readonly ILoggingService loggingService;
        private IList<KeyValuePair<string, object>> parameterInfo;

        public DataService(NpgsqlConnection connection, ILoggingService loggingService)
        {
            this.connection = connection;
            this.loggingService = loggingService;
            this.parameterInfo = null;
        }

        public async Task<string> GetStringAsync(string command, Action<NpgsqlParameterCollection> parameters)
        {
            connection.Notice += loggingService.CreateNoticeEventHandler(command);
            await EnsureConnectionIsOpen();
            using (var cmd = new NpgsqlCommand(command, connection))
            {
                parameters?.Invoke(cmd.Parameters);
                return await GetStringContentFromCommand(cmd);
            }
        }

        public async Task<string> GetStringAsync(string command, Func<NpgsqlParameterCollection, Task> parameters)
        {
            connection.Notice += loggingService.CreateNoticeEventHandler(command);
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
            connection.Notice += loggingService.CreateNoticeEventHandler(command);
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

        public IList<KeyValuePair<string, object>> GetParameterInfo() => this.parameterInfo;

        private async Task<string> GetStringContentFromCommand(NpgsqlCommand cmd)
        {
            if (cmd.Parameters != null && cmd.Parameters.Count > 0)
            {
                this.parameterInfo = cmd.Parameters.Select(p => new KeyValuePair<string, object>(p.ParameterName, p.NpgsqlValue)).ToList();
            }
            else
            {
                this.parameterInfo = null;
            }    
            using (var reader = cmd.ExecuteReader())
            {
                if (!await reader.ReadAsync())
                {
                    return null;
                }
                var value = reader.GetValue(0);
                if (value == DBNull.Value)
                {
                    return null;
                }
                if (value.GetType() == typeof(string))
                {
                    return value as string;
                }
                else if (value.GetType() == typeof(DateTime))
                {
                    DateTime dt = (DateTime)value;
                    return dt.ToString("s");
                }
                return Convert.ToString(value);
            }
        }
    }
}
