using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace UnitTests
{
    public class RestClient
    {
        public static async Task<(TResult, HttpStatusCode, string)> GetAsync<TResult>(string url) where TResult : class
        {
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                TResult result;
                var responseJson = await response.Content.ReadAsStringAsync();
                if (typeof(TResult) == typeof(string))
                {
                    result = responseJson as TResult;
                } else
                {
                    result = JsonConvert.DeserializeObject<TResult>(responseJson);
                }
                return (result, response.StatusCode, response.Content.Headers?.ContentType?.ToString());
            }
        }
    }
}
