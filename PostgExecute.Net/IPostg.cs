using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;

namespace PostgExecute.Net
{
    public interface IPostg
    {
        NpgsqlConnection Connection { get; }
        IPostg Execute(string command);
        IPostg Execute(string command, params object[] parameters);
        IPostg Execute(string command, Action<NpgsqlParameterCollection> parameters);
        Task<IPostg> ExecuteAsync(string command);
        Task<IPostg> ExecuteAsync(string command, params object[] parameters);
        Task<IPostg> ExecuteAsync(string command, Action<NpgsqlParameterCollection> parameters);
        Task<IPostg> ExecuteAsync(string command, Func<NpgsqlParameterCollection, Task> parameters);

        IDictionary<string, object> Single(string command);
        IDictionary<string, object> Single(string command, params object[] parameters);
        IDictionary<string, object> Single(string command, Action<NpgsqlParameterCollection> parameters);
        Task<IDictionary<string, object>> SingleAsync(string command);
        Task<IDictionary<string, object>> SingleAsync(string command, params object[] parameters);
        Task<IDictionary<string, object>> SingleAsync(string command, Action<NpgsqlParameterCollection> parameters);
        Task<IDictionary<string, object>> SingleAsync(string command, Func<NpgsqlParameterCollection, Task> parameters);

        IEnumerable<IDictionary<string, object>> Read(string command);
        IEnumerable<IDictionary<string, object>> Read(string command, params object[] parameters);
        IEnumerable<IDictionary<string, object>> Read(string command, Action<NpgsqlParameterCollection> parameters);
        Task ReadAsync(string command, Action<IDictionary<string, object>> results);
        Task ReadAsync(string command, Action<IDictionary<string, object>> results, params object[] parameters);
        Task ReadAsync(string command, Action<IDictionary<string, object>> results, Action<NpgsqlParameterCollection> parameters);
        Task ReadAsync(string command, Action<IDictionary<string, object>> results, Func<NpgsqlParameterCollection, Task> parameters);
        Task ReadAsync(string command, Func<IDictionary<string, object>, Task> results);
        Task ReadAsync(string command, Func<IDictionary<string, object>, Task> results, params object[] parameters);
        Task ReadAsync(string command, Func<IDictionary<string, object>, Task> results, Action<NpgsqlParameterCollection> parameters);
        Task ReadAsync(string command, Func<IDictionary<string, object>, Task> results, Func<NpgsqlParameterCollection, Task> parameters);
    }
}