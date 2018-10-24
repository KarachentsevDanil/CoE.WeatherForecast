using System.Threading.Tasks;
using WheatherForecast.Provider.Dto;

namespace WheatherForecast.Provider.Api
{
    public interface IWheatherForecastApi
    {
        Task<WheaterForecastDto> GetWheaterForecast(string place);
    }
}
