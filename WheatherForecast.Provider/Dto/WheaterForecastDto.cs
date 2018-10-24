namespace WheatherForecast.Provider.Dto
{
    public class WheaterForecastDto
    {
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
