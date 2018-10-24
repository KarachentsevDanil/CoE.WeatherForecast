using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherForecast.Provider;
using WeatherForecast.Provider.Configuration;
using WeatherForecast.Provider.Constants;

namespace WeatherForecast.Spa
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddHttpClient(WeatherApiConstants.HttpClientName, client =>
            {
                client.BaseAddress = new Uri(Configuration["WeatherForecastApi:BaseApiUrl"]);
                client.Timeout = TimeSpan.FromSeconds(WeatherApiConstants.RequestTimeout);
            });

            services.AddSingleton<IWeatherForecastConfiguration, WeatherForecastConfiguration>(serviceProvider
                => new WeatherForecastConfiguration(Configuration["WeatherForecastApi:ApiKey"], Configuration["WeatherForecastApi:GetWeatherForecastEndpoint"]));

            services.AddWeatherForecastApiDependencies();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
