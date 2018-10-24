using Microsoft.Extensions.DependencyInjection;
using WeatherForecast.Provider.Api;

namespace WeatherForecast.Provider
{
    public static class BootstrapInjection
    {
        public static IServiceCollection AddWeatherForecastApiDependencies(this IServiceCollection services)
        {
            services.AddTransient<IWeatherForecastApi, WeatherForecastApi>();

            return services;
        }
    }
}
