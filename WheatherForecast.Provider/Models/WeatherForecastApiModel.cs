using Newtonsoft.Json;
using System.Collections.Generic;
using WeatherForecast.Provider.Dto;

namespace WeatherForecast.Provider.Models
{
    public class WeatherForecastApiModel
    {
        public List<WeatherApiModel> Weather { get; set; }
        
        [JsonProperty(PropertyName = "main")]
        public TemperatureApiModel Temperature { get; set; }

        [JsonProperty(PropertyName = "sys")]
        public SystemInfoApiModel SystemInfo { get; set; }

        public WindApiModel Wind { get; set; }

        public CloudsApiModel Clouds { get; set; }
        
        [JsonProperty(PropertyName = "name")]
        public string Place { get; set; }

        public WeatherForecastDto ToWeatherForecastDto()
        {
            var weatherForecastDto = new WeatherForecastDto(this);
            return weatherForecastDto;
        }
    }

    public class WeatherApiModel
    {
        [JsonProperty(PropertyName = "temp")]
        public string Weather { get; set; }

        public string Description { get; set; }

        public string Icon { get; set; }
    }

    public class TemperatureApiModel
    {
        [JsonProperty(PropertyName = "temp")]
        public double Temperature { get; set; }

        public int Pressure { get; set; }

        public int Humidity { get; set; }

        [JsonProperty(PropertyName = "temp_min")]
        public double MaxTemperature { get; set; }

        [JsonProperty(PropertyName = "temp_max")]
        public double MinTemperature { get; set; }
    }

    public class WindApiModel
    {
        public double Speed { get; set; }
    }

    public class CloudsApiModel
    {
        [JsonProperty(PropertyName = "all")]
        public int Overcast { get; set; }
    }

    public class SystemInfoApiModel
    {
        public string Country { get; set; }

        public long Sunrise { get; set; }

        public long Sunset { get; set; }
    }
}
