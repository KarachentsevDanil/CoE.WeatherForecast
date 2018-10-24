using System.Threading.Tasks;
using WheatherForecast.Provider.Dto;

namespace WheatherForecast.Provider.Api
{
    public class WheatherForecastApi : IWheatherForecastApi
    {
        public Task<WheaterForecastDto> GetWheaterForecast(string place)
        {
            throw new System.NotImplementedException();
        }
    }
}
