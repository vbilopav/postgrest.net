using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace UnitTests
{
    public class RestClient
    {
        public static async Task<(TResult, HttpStatusCode, string)> GetAsync<TResult>(string url, HttpClient client = null)
            where TResult : class =>
            await RestActionAsync<TResult>(async c => await c.GetAsync(url, HttpCompletionOption.ResponseHeadersRead), client);

        public static async Task<(TResult, HttpStatusCode, string)> PostAsync<TResult>(string url, HttpContent content, HttpClient client = null)
            where TResult : class =>
            await RestActionAsync<TResult>(async c => await c.PostAsync(url, content), client);

        public static async Task<(TResult, HttpStatusCode, string)> PutAsync<TResult>(string url, HttpContent content, HttpClient client = null)
            where TResult : class =>
            await RestActionAsync<TResult>(async c => await c.PutAsync(url, content), client);

        public static async Task<(TResult, HttpStatusCode, string)> DeleteAsync<TResult>(string url, HttpClient client = null) where TResult : class =>
            await RestActionAsync<TResult>(async c => await c.DeleteAsync(url), client);

        private static async Task<(TResult, HttpStatusCode, string)> RestActionAsync<TResult>(
            Func<HttpClient, Task<HttpResponseMessage>> func, HttpClient client = null)
            where TResult : class
        {
            async Task<(TResult, HttpStatusCode, string)> ExecuteAction(HttpClient c)
            {
                using (var response = await func(c))
                {
                    TResult result;
                    var responseJson = await response.Content.ReadAsStringAsync();
                    if (typeof(TResult) == typeof(string))
                    {
                        result = responseJson as TResult;
                    }
                    else
                    {
                        result = JsonConvert.DeserializeObject<TResult>(responseJson);
                    }
                    return (result, response.StatusCode, response.Content.Headers?.ContentType?.ToString());
                }
            }
            if (client != null)
            {
                return await ExecuteAction(client);
            }
            using (var c = new HttpClient())
            {
                return await ExecuteAction(c);
            }
        }
    }
}
