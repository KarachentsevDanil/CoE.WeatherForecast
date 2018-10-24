using System.Threading.Tasks;
using WeatherForecast.Provider.Dto;
using WeatherForecast.Provider.Models;

namespace WeatherForecast.Provider.Api
{
    public interface IWeatherForecastApi
    {
        Task<WeatherForecastDto> GetWeatherForecast(WeatherForecastSearchModel model);
    }
}
