using System;
using System.Linq;
using WeatherForecast.Provider.Constants;
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
            Country = weatherForecastApiModel.SystemInfo.Country;
            Sunrise = new DateTime().AddSeconds(weatherForecastApiModel.SystemInfo.Sunrise).ToLocalTime().ToShortTimeString();
            Sunset = new DateTime().AddSeconds(weatherForecastApiModel.SystemInfo.Sunset).ToLocalTime().ToShortTimeString();

            if (weatherForecastApiModel.Weather != null && weatherForecastApiModel.Weather.Any())
            {
                var weatherDescription = weatherForecastApiModel.Weather.FirstOrDefault();
                WeatherDescription = weatherDescription.Description;
                Icon = weatherDescription.Icon;
            }
        }

        public void SetImageUrl(string baseImageUrl)
        {
            ImageUrl = $"{baseImageUrl}{Icon}{WeatherApiConstants.ImageExtension}";
        }

        public void SetCountryImageUrl(string baseImageUrl)
        {
            CountryImage = $"{baseImageUrl}{Country.ToLower()}{WeatherApiConstants.ImageExtension}";
        }

        public string Place { get; set; }

        public double Temperature { get; set; }

        public double MinTemperature { get; set; }

        public double MaxTemperature { get; set; }
        
        public int Pressure { get; set; }

        public int Humidity { get; set; }

        public double WindSpeed { get; set; }

        public int Overcast { get; set; }

        public string WeatherDescription { get; set; }

        public string Icon { get; set; }

        public string ImageUrl { get; set; }

        public string Country { get; set; }

        public string Sunrise { get; set; }

        public string Sunset { get; set; }

        public string CountryImage { get; set; }
    }
}
