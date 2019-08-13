using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PostgRest.net
{
    public static class PostgRestExtensions
    {
        public static string GetPgCloudConnectionString(this IConfiguration config, string connectionStringName) =>
            config.GetConnectionString(connectionStringName) ?? config.GetSection($"POSTGRESQLCONNSTR_{connectionStringName}")?.Value;

        public static string RemoveFromStart(this string source, string remove) =>
            source.Remove(source.IndexOf(remove, StringComparison.Ordinal), remove.Length);

        public static JObject ToJObject(this IQueryCollection queryCollection)
        {
            var result = new JObject();
            foreach (var query in queryCollection)
            {
                result[query.Key] = query.Value.ToString();
            }
            return result;
        }

        public static async Task<string> GetBodyAsync(this HttpRequest request)
        {
            using (var reader = new StreamReader(request.Body, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
