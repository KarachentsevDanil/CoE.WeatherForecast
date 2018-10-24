using Newtonsoft.Json;

namespace WeatherForecast.Provider.Extensions
{
    public static class StringExtensions
    {
        public static T TryParseJson<T>(this string json) where T : new()
        {
            try
            {
                var deserializedObject = JsonConvert.DeserializeObject<T>(json);
                return deserializedObject;
            }
            catch (JsonException)
            {
                return default(T);
            }
        }
    }
}
