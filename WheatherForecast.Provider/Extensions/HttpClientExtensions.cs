using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast.Provider.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<T> GetData<T>(this HttpClient client, string address) where T : class, new()
        {
            try
            {
                var response = await client.GetAsync(address);

                var responseText = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
                var result = responseText.TryParseJson<T>();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.GetBaseException().Message);
            }
        }
    }

    public class ParameterCollection
    {
        private readonly Dictionary<string, string> _params = new Dictionary<string, string>();

        public void Add(string key, string val)
        {
            if (_params.ContainsKey(key))
            {
                return;
            }

            _params.Add(key, val);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            foreach (var item in _params)
            {
                if (builder.Length > 0)
                {
                    builder.Append("&");
                }

                builder.Append($"{item.Key}={item.Value}");
            }

            return builder.ToString();
        }
    }
}
