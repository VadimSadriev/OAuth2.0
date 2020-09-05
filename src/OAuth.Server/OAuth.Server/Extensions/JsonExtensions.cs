using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OAuth.Server.Extensions
{
    public static class JsonExtensions
    {
        private static readonly JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        static JsonExtensions()
        {
            DefaultJsonOptions.Converters.Add(new JsonStringEnumConverter());
        }
       
        public static string Serialize(this object obj, JsonSerializerOptions? jsonSerializerOptions = null)
        {
            return JsonSerializer.Serialize(obj, jsonSerializerOptions ?? DefaultJsonOptions);
        }

      
        public static TModel Deserialize<TModel>(this string json, JsonSerializerOptions? jsonSerializerOptions = null)
        {
            return JsonSerializer.Deserialize<TModel>(json, jsonSerializerOptions ?? DefaultJsonOptions);
        }
    }
}
