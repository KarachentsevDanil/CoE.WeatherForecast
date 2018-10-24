using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeatherForecast.Provider.Api;
using WeatherForecast.Provider.Dto;
using WeatherForecast.Provider.Models;

namespace WeatherForecast.Spa.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherForecastApi _weatherForecastApi;

        public WeatherController(IWeatherForecastApi weatherForecastApi)
        {
            _weatherForecastApi = weatherForecastApi;
        }

        [HttpGet]
        public async  Task<ActionResult<WeatherForecastDto>> GetWeatherForecast(string name, string unit)
        {
            var model = new WeatherForecastSearchModel(name, unit);
            var result = await _weatherForecastApi.GetWeatherForecast(model);
            return result;
        }
    }
}
