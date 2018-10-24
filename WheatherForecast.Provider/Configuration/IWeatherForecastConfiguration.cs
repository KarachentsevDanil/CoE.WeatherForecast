namespace WeatherForecast.Provider.Configuration
{
    public interface IWeatherForecastConfiguration
    {
        string ApiKey { get; set; }

        string GetWeatherForecast { get; set; }

        string ImageUrl { get; set; }
    }
}
