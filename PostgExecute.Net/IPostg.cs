using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;

namespace PostgExecute.Net
{
    public interface IPostg : IPostgConnection, 
        IPostgExecute, IPostgExecuteAsync, 
        IPostgSingle, IPostgSingleAsync,
        IPostgRead, IPostgReadResults, IPostgReadResultsConditional, 
        IPostgReadResultsAsync, IPostgReadAsyncResultsAsync,
        IPostgReadResultsConditionalAsync, IPostgReadAsyncResultsConditionalAsync { }

    public interface IPostgConnection
    {
        NpgsqlConnection Connection { get; }
    }

    public interface IPostgExecute
    {
        IPostg Execute(string command);
        IPostg Execute(string command, params object[] parameters);
        IPostg Execute(string command, params (string name, object value)[] parameters);
        IPostg Execute(string command, Action<NpgsqlParameterCollection> parameters);
    }

    public interface IPostgExecuteAsync
    {
        Task<IPostg> ExecuteAsync(string command);
        Task<IPostg> ExecuteAsync(string command, params object[] parameters);
        Task<IPostg> ExecuteAsync(string command, params (string name, object value)[] parameters);
        Task<IPostg> ExecuteAsync(string command, Action<NpgsqlParameterCollection> parameters);
        Task<IPostg> ExecuteAsync(string command, Func<NpgsqlParameterCollection, Task> parameters);
    }

    public interface IPostgSingle
    {
        IDictionary<string, object> Single(string command);
        IDictionary<string, object> Single(string command, params object[] parameters);
        IDictionary<string, object> Single(string command, params (string name, object value)[] parameters);
        IDictionary<string, object> Single(string command, Action<NpgsqlParameterCollection> parameters);
    }

    public interface IPostgSingleAsync
    {
        Task<IDictionary<string, object>> SingleAsync(string command);
        Task<IDictionary<string, object>> SingleAsync(string command, params object[] parameters);
        Task<IDictionary<string, object>> SingleAsync(string command, params (string name, object value)[] parameters);
        Task<IDictionary<string, object>> SingleAsync(string command, Action<NpgsqlParameterCollection> parameters);
        Task<IDictionary<string, object>> SingleAsync(string command, Func<NpgsqlParameterCollection, Task> parameters);
    }

    public interface IPostgRead
    {
        IEnumerable<IDictionary<string, object>> Read(string command);
        IEnumerable<IDictionary<string, object>> Read(string command, params object[] parameters);
        IEnumerable<IDictionary<string, object>> Read(string command, params (string name, object value)[] parameters);
        IEnumerable<IDictionary<string, object>> Read(string command, Action<NpgsqlParameterCollection> parameters);
    }

    public interface IPostgReadResults
    {
        IPostg Read(string command, Action<IDictionary<string, object>> results);
        IPostg Read(string command, Action<IDictionary<string, object>> results, params object[] parameters);
        IPostg Read(string command, Action<IDictionary<string, object>> results, params (string name, object value)[] parameters);
        IPostg Read(string command, Action<IDictionary<string, object>> results, Action<NpgsqlParameterCollection> parameters);
    }

    public interface IPostgReadResultsConditional
    {
        IPostg Read(string command, Func<IDictionary<string, object>, bool> results);
        IPostg Read(string command, Func<IDictionary<string, object>, bool> results, params object[] parameters);
        IPostg Read(string command, Func<IDictionary<string, object>, bool> results, params (string name, object value)[] parameters);
        IPostg Read(string command, Func<IDictionary<string, object>, bool> results, Action<NpgsqlParameterCollection> parameters);
    }

    public interface IPostgReadResultsAsync
    {
        Task<IPostg> ReadAsync(string command, Action<IDictionary<string, object>> results);
        Task<IPostg> ReadAsync(string command, Action<IDictionary<string, object>> results, params object[] parameters);
        Task<IPostg> ReadAsync(string command, Action<IDictionary<string, object>> results, params (string name, object value)[] parameters);
        Task<IPostg> ReadAsync(string command, Action<IDictionary<string, object>> results, Action<NpgsqlParameterCollection> parameters);
        Task<IPostg> ReadAsync(string command, Action<IDictionary<string, object>> results, Func<NpgsqlParameterCollection, Task> parameters);
    }

    public interface IPostgReadAsyncResultsAsync
    {
        Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, Task> results);
        Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, Task> results, params object[] parameters);
        Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, Task> results, params (string name, object value)[] parameters);
        Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, Task> results, Action<NpgsqlParameterCollection> parameters);
        Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, Task> results, Func<NpgsqlParameterCollection, Task> parameters);
    }


    public interface IPostgReadResultsConditionalAsync
    {
        Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, bool> results);
        Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, bool> results, params object[] parameters);
        Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, bool> results, params (string name, object value)[] parameters);
        Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, bool> results, Action<NpgsqlParameterCollection> parameters);
        Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, bool> results, Func<NpgsqlParameterCollection, Task> parameters);

    }

    public interface IPostgReadAsyncResultsConditionalAsync
    { 
        Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, Task<bool>> results);
        Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, Task<bool>> results, params object[] parameters);
        Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, Task<bool>> results, params (string name, object value)[] parameters);
        Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, Task<bool>> results, Action<NpgsqlParameterCollection> parameters);
        Task<IPostg> ReadAsync(string command, Func<IDictionary<string, object>, Task<bool>> results, Func<NpgsqlParameterCollection, Task> parameters);
    }
}