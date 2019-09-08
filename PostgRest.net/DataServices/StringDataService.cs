using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Npgsql;

namespace PostgRest.Net.DataServices
{
    public interface IStringDataService
    {
        Task<string> GetStringAsync(string command, Action<NpgsqlParameterCollection> parameters, bool fromRecordSet = false);
        Task<string> GetStringAsync(string command, Func<NpgsqlParameterCollection, Task> parameters, bool fromRecordSet = false);
        Task<string> GetStringAsync(string command, bool fromRecordSet = false);
        IList<KeyValuePair<string, object>> GetParameterInfo();
    }

    public class StringDataService : IStringDataService
    {
        private readonly NpgsqlConnection connection;
        private readonly ILoggingService loggingService;
        private IList<KeyValuePair<string, object>> parameterInfo;

        public StringDataService(NpgsqlConnection connection, ILoggingService loggingService)
        {
            this.connection = connection;
            this.loggingService = loggingService;
            this.parameterInfo = null;
        }

        public async Task<string> GetStringAsync(string command, Action<NpgsqlParameterCollection> parameters, bool fromRecordSet = false)
        {
            connection.Notice += loggingService.CreateNoticeEventHandler(command);
            await EnsureConnectionIsOpenAsync();
            using (var cmd = new NpgsqlCommand(command, connection))
            {
                parameters?.Invoke(cmd.Parameters);
                return await GetStringContentFromCommand(cmd, fromRecordSet);
            }
        }

        public async Task<string> GetStringAsync(string command, Func<NpgsqlParameterCollection, Task> parameters, bool fromRecordSet = false)
        {
            connection.Notice += loggingService.CreateNoticeEventHandler(command);
            await EnsureConnectionIsOpenAsync();
            using (var cmd = new NpgsqlCommand(command, connection))
            {
                if (parameters != null)
                {
                    await parameters?.Invoke(cmd.Parameters);
                }
                return await GetStringContentFromCommand(cmd, fromRecordSet);
            }
        }

        public async Task<string> GetStringAsync(string command, bool fromRecordSet = false)
        {
            connection.Notice += loggingService.CreateNoticeEventHandler(command);
            await EnsureConnectionIsOpenAsync();
            using (var cmd = new NpgsqlCommand(command, connection))
            {
                return await GetStringContentFromCommand(cmd, fromRecordSet);
            }
        }

        private async Task EnsureConnectionIsOpenAsync()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }
        }

        public IList<KeyValuePair<string, object>> GetParameterInfo() => this.parameterInfo;

        private async Task<string> GetStringContentFromCommand(NpgsqlCommand cmd, bool fromRecordSet = false)
        {
            ExtractParamsInfo(cmd);
            if (!fromRecordSet)
            {
                return await GetStringValueFromCommand(cmd);
            }
            else
            {
                return await GetStringRecordsFromCommand(cmd);
            }
        }
        private static async Task<string> GetStringValueFromCommand(NpgsqlCommand cmd)
        {
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
                switch (value)
                {
                    case string s:
                        return s;
                    case DateTime dt:
                        return dt.ToString("s");
                    default:
                        return Convert.ToString(value);
                }
            }
        }

        private static async Task<string> GetStringRecordsFromCommand(NpgsqlCommand cmd)
        {
            var result = new List<IDictionary<string, object>>();
            using (var reader = cmd.ExecuteReader())
            {
                while(await reader.ReadAsync())
                {
                    var record = new Dictionary<string, object>();
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        record[reader.GetName(i)] = reader.GetValue(i);
                    }
                    result.Add(record);
                }
            }
            return JsonConvert.SerializeObject(result);
        }

        private void ExtractParamsInfo(NpgsqlCommand cmd)
        {
            if (cmd.Parameters != null && cmd.Parameters.Count > 0)
            {
                this.parameterInfo = cmd.Parameters.Select(p => new KeyValuePair<string, object>(p.ParameterName, p.NpgsqlValue)).ToList();
            }
            else
            {
                this.parameterInfo = null;
            }
        }
    }
}
