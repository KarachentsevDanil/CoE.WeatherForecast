using Newtonsoft.Json;
using System.Collections.Generic;

namespace WheatherForecast.Provider.Models
{
    public class WheaterForecastApiModel
    {
        public List<WeatherApiModel> Weather { get; set; }

        [JsonProperty(PropertyName = "temp")]
        public TemperatureApiModel Temperature { get; set; }
        
        public WindApiModel Wind { get; set; }

        public CloudsApiModel Clouds { get; set; }
        
        [JsonProperty(PropertyName = "name")]
        public string Place { get; set; }
    }

    public class WeatherApiModel
    {
        [JsonProperty(PropertyName = "temp")]
        public string Wheater { get; set; }

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
}
