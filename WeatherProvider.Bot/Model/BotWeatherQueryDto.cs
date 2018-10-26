using WeatherForecast.Provider.Models;

namespace WeatherProvider.Bot.Model
{
    public class BotWeatherQueryDto
    {
        public string City { get; set; }

        public string Units { get; set; }

        public WeatherForecastSearchModel ToWeatherForecastDto()
        {
            return new WeatherForecastSearchModel(City, WeatherForecastSearchModel.GetUnitValueForApi(Units.ToLower()));
        }

        public string GetDisplayUnitsValue()
        {
            switch (Units)
            {
                case "1": return "°С";
                case "2": return "°F";
                case "Celsius": return "°С";
                case "Fahrenheit": return "°F";
                default: return "°K";
            }
        }
    }
}
