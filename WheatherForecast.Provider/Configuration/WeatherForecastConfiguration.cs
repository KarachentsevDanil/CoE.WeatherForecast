namespace WeatherForecast.Provider.Configuration
{
    public class WeatherForecastConfiguration : IWeatherForecastConfiguration
    {
        public WeatherForecastConfiguration(string apiKey, string getWeatherForecast, string imageUrl)
        {
            ApiKey = apiKey;
            GetWeatherForecast = getWeatherForecast;
            ImageUrl = imageUrl;
        }

        public string ApiKey { get; set; }

        public string GetWeatherForecast { get; set; }

        public string ImageUrl { get; set; }
    }
}
