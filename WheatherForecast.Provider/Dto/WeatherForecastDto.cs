using WeatherForecast.Provider.Models;

namespace WeatherForecast.Provider.Dto
{
    public class WeatherForecastDto
    {
        public WeatherForecastDto()
        {
            
        }

        public WeatherForecastDto(WeatherForecastApiModel weatherForecastApiModel)
        {
            Place = weatherForecastApiModel.Place;
            Temperature = weatherForecastApiModel.Temperature.Temperature;
            MinTemperature = weatherForecastApiModel.Temperature.MinTemperature;
            MaxTemperature = weatherForecastApiModel.Temperature.MaxTemperature;
            Pressure = weatherForecastApiModel.Temperature.Pressure;
            Humidity = weatherForecastApiModel.Temperature.Humidity;
            WindSpeed = weatherForecastApiModel.Wind.Speed;
            Overcast = weatherForecastApiModel.Clouds.Overcast;
        }

        public string Place { get; set; }

        public double Temperature { get; set; }

        public double MinTemperature { get; set; }

        public double MaxTemperature { get; set; }
        
        public int Pressure { get; set; }

        public int Humidity { get; set; }

        public double WindSpeed { get; set; }

        public int Overcast { get; set; }
    }
}
