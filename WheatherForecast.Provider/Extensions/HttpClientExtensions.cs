using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WheatherForecast.Provider.Constants;

namespace WheatherForecast.Provider.Extensions
{
    public static class HttpClientExtensions
    {
        public static void SetAuthenticationHeader(this HttpClient client, string schema, string headerValue)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(schema, headerValue);
        }

        public static void SetDefaultRequestHeaders(this HttpClient client, string headerType)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(headerType));
        }

        public static async Task PostDataAsJson<T>(this HttpClient client, T data, string address)
        {
            var jsonData = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var content = new StringContent(jsonData, Encoding.UTF8, MediaTypeConstants.JsonMediaType);
            await PostData(client, address, content);
        }

        public static async Task<TR> PostDataAsJson<TI, TR>(this HttpClient client, TI data, string address) where TR : class, new()
        {
            var jsonData = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var content = new StringContent(jsonData, Encoding.UTF8, MediaTypeConstants.JsonMediaType);
            return await PostData<TR>(client, address, content);
        }

        public static async Task PostData<T>(this HttpClient client, T data, string address, string contentType)
        {
            var content = new StringContent(data.ToString(), Encoding.UTF8, contentType);
            await PostData(client, address, content);
        }

        public static async Task<TR> PostData<TI, TR>(this HttpClient client, TI data, string address, string contentType) where TR : class, new()
        {
            var content = new StringContent(data.ToString(), Encoding.UTF8, contentType);
            return await PostData<TR>(client, address, content);
        }

        public static async Task<T> GetData<T>(this HttpClient client, string address) where T : class, new()
        {
            HttpResponseMessage response;

            try
            {
                response = await client.GetAsync(address);

                var responseText = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
                var result = responseText.TryParseJson<T>();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.GetBaseException().Message);
            }
        }

        private static async Task<HttpResponseMessage> PostDataAndGetHttpResponse(this HttpClient client, HttpContent content, string address)
        {
            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync(address, content);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.GetBaseException().Message);
            }

            return response;
        }

        private static async Task PostData(this HttpClient client, string address, HttpContent content)
        {
            (await PostDataAndGetHttpResponse(client, content, address)).EnsureSuccessStatusCode();
        }

        private static async Task<T> PostData<T>(this HttpClient client, string address, HttpContent content) where T : class, new()
        {
            var response = (await PostDataAndGetHttpResponse(client, content, address)).EnsureSuccessStatusCode();

            var responseText = await response.Content.ReadAsStringAsync();
            var result = responseText.TryParseJson<T>();

            return result;
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
