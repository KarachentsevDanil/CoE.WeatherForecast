using WeatherForecast.Provider.Constants;

namespace WeatherForecast.Provider.Models
{
    public class WeatherForecastSearchModel
    {
        public WeatherForecastSearchModel(string name, string units)
        {
            PlaceName = name;
            Units = string.IsNullOrEmpty(units) ? WeatherApiConstants.DefaultTemperatureUnits : units;
        }

        public string PlaceName { get; set; }

        public string Units { get; set; }
    }
}
