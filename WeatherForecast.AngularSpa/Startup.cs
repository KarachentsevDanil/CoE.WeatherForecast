using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherForecast.Provider;
using WeatherForecast.Provider.Configuration;
using WeatherForecast.Provider.Constants;

namespace WeatherForecast.AngularSpa
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

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddHttpClient(WeatherApiConstants.HttpClientName, client =>
            {
                client.BaseAddress = new Uri(Configuration["WeatherForecastApi:BaseApiUrl"]);
                client.Timeout = TimeSpan.FromSeconds(WeatherApiConstants.RequestTimeout);
            });

            services.AddSingleton<IWeatherForecastConfiguration, WeatherForecastConfiguration>(serviceProvider
                => new WeatherForecastConfiguration(
                    Configuration["WeatherForecastApi:ApiKey"],
                    Configuration["WeatherForecastApi:GetWeatherForecastEndpoint"],
                    Configuration["WeatherForecastApi:ImageUrl"],
                    Configuration["WeatherForecastApi:CountryImageUrl"]));

            services.AddWeatherForecastApiDependencies();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
