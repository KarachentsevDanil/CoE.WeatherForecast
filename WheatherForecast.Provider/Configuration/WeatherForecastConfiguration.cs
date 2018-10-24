namespace WeatherForecast.Provider.Configuration
{
    public class WeatherForecastConfiguration : IWeatherForecastConfiguration
    {
        public WeatherForecastConfiguration(string apiKey, string getWeatherForecast)
        {
            ApiKey = apiKey;
            GetWeatherForecast = getWeatherForecast;
        }

        public string ApiKey { get; set; }
        public string GetWeatherForecast { get; set; }
    }
}
