namespace WeatherForecast.Provider.Configuration
{
    public class WeatherForecastConfiguration : IWeatherForecastConfiguration
    {
        public WeatherForecastConfiguration(string apiKey, string getWeatherForecast, string imageUrl, string countryImageUrl)
        {
            ApiKey = apiKey;
            GetWeatherForecast = getWeatherForecast;
            ImageUrl = imageUrl;
            CountryImageUrl = countryImageUrl;
        }

        public string ApiKey { get; set; }

        public string GetWeatherForecast { get; set; }

        public string ImageUrl { get; set; }

        public string CountryImageUrl { get; set; }
    }
}
