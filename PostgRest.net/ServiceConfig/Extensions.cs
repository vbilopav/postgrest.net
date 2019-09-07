using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;

namespace PostgRest.Net.ServiceConfig
{
    public static class Extensions
    {
        public static string GetPgCloudConnectionString(this IConfiguration config, string connectionStringName) =>
            config.GetConnectionString(connectionStringName) ?? config.GetSection($"POSTGRESQLCONNSTR_{connectionStringName}")?.Value;

        public static string RemoveFromStart(this string source, string remove) => 
            !source.StartsWith(remove) ? source : source.Remove(0, remove.Length);

        public static JObject ToJObject(this IEnumerable<KeyValuePair<string, StringValues>> queryCollection)
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
