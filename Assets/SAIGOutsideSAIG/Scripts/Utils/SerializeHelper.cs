using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Utils
{
    public static class SerializeHelper
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Converters = new JsonConverter[] { new StringEnumConverter() }
        };

        public static T Deserialize<T>(string input)
        {
            if (typeof(T) == typeof(string)) return (T)(object)input;
            return JsonConvert.DeserializeObject<T>(input, Settings);
        }

        public static string Serialize<T>(T input)
        {
            if (typeof(T) == typeof(string)) return (string)(object)input;
            return JsonConvert.SerializeObject(input, Settings);
        }
    }
}