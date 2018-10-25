using System.Net.Http;
using System.Threading.Tasks;
using WeatherForecast.Provider.Configuration;
using WeatherForecast.Provider.Constants;
using WeatherForecast.Provider.Dto;
using WeatherForecast.Provider.Extensions;
using WeatherForecast.Provider.Models;

namespace WeatherForecast.Provider.Api
{
    public class WeatherForecastApi : IWeatherForecastApi
    {
        private readonly IWeatherForecastConfiguration _configuration;
        private readonly HttpClient _client;

        public WeatherForecastApi(IWeatherForecastConfiguration configuration, IHttpClientFactory factory)
        {
            _configuration = configuration;
            _client = factory.CreateClient(WeatherApiConstants.HttpClientName);
        }

        public async Task<WeatherForecastDto> GetWeatherForecast(WeatherForecastSearchModel model)
        {
            var parameters = BuildParameters(model);

            var response = await _client.GetData<WeatherForecastApiModel>($"{_configuration.GetWeatherForecast}?{parameters}");

            var weatherForecastDto = response.ToWeatherForecastDto();

            weatherForecastDto.SetImageUrl(_configuration.ImageUrl);
            weatherForecastDto.SetCountryImageUrl(_configuration.CountryImageUrl);

            return weatherForecastDto;
        }

        private ParameterCollection BuildParameters(WeatherForecastSearchModel model)
        {
            var parameters = new ParameterCollection();

            parameters.Add(QueryConstants.ApiKey, _configuration.ApiKey);
            parameters.Add(QueryConstants.PlaceName, model.PlaceName);
            parameters.Add(QueryConstants.TemperatureType, model.Units);

            return parameters;
        }
    }
}
