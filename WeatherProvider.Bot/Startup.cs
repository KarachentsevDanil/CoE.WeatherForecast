using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherForecast.Provider;
using WeatherForecast.Provider.Configuration;
using WeatherForecast.Provider.Constants;
using WeatherProvider.Bot.Bot;
using WeatherProvider.Bot.Model;
using WeatherProvider.Bot.State;

namespace WeatherProvider.Bot
{
    public class Startup
    {
        private bool _isProduction = false;
        private ILoggerFactory _loggerFactory;

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
                => new WeatherForecastConfiguration(
                    Configuration["WeatherForecastApi:ApiKey"],
                    Configuration["WeatherForecastApi:GetWeatherForecastEndpoint"],
                    Configuration["WeatherForecastApi:ImageUrl"],
                    Configuration["WeatherForecastApi:CountryImageUrl"]));

            services.AddWeatherForecastApiDependencies();

            services.AddSingleton(sp =>
            {
                var luisApp = new LuisApplication(
                    applicationId: Configuration["LuisAI:ApiKey"],
                    endpointKey: Configuration["LuisAI:EndpointKey"],
                    endpoint: Configuration["LuisAI:EndpointUrl"]);

                var luisPredictionOptions = new LuisPredictionOptions
                {
                    IncludeAllIntents = true
                };

                return new LuisRecognizer(luisApp, luisPredictionOptions, true);
            });

            services.AddBot<WeatherBot>(options =>
            {
                var secretKey = Configuration["botFileSecret"];
                var botFilePath = Configuration["botFilePath"];

                // Loads .bot configuration file and adds a singleton that your Bot can access through dependency injection.
                var botConfig = BotConfiguration.Load(botFilePath ?? @".\BotConfiguration.bot", secretKey);
                services.AddSingleton(sp => botConfig ?? throw new InvalidOperationException($"The .bot configuration file could not be loaded."));

                // Retrieve current endpoint.
                var environment = _isProduction ? "production" : "development";
                var service = botConfig.Services.FirstOrDefault(s => s.Type == "endpoint" && s.Name == environment);

                if (!(service is EndpointService endpointService))
                {
                    throw new InvalidOperationException($"The .bot file does not contain an endpoint with name '{environment}'.");
                }

                options.CredentialProvider = new SimpleCredentialProvider(endpointService.AppId, endpointService.AppPassword);

                // Creates a logger for the application to use.
                ILogger logger = _loggerFactory.CreateLogger<WeatherBot>();

                // Catches any errors that occur during a conversation turn and logs them.
                options.OnTurnError = async (context, exception) =>
                {
                    logger.LogError($"Exception caught : {exception}");
                    await context.SendActivityAsync("Sorry, it looks like something went wrong.");
                };

                // The Memory Storage used here is for local bot debugging only. When the bot
                // is restarted, everything stored in memory will be gone.
                IStorage dataStore = new MemoryStorage();

                // Create Conversation State object.
                // The Conversation State object is where we persist anything at the conversation-scope.
                var conversationState = new ConversationState(dataStore);
                options.State.Add(conversationState);

                // Create and add user state.
                var userState = new UserState(dataStore);
                options.State.Add(userState);
            });

            services.AddSingleton<WeatherBotAccessors>(sp =>
            {
                // We need to grab the conversationState we added on the options in the previous step
                var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;
                if (options == null)
                {
                    throw new InvalidOperationException("BotFrameworkOptions must be configured prior to setting up the State Accessors");
                }

                var conversationState = options.State.OfType<ConversationState>().FirstOrDefault();
                if (conversationState == null)
                {
                    throw new InvalidOperationException("ConversationState must be defined and added before adding conversation-scoped state accessors.");
                }

                var userState = options.State.OfType<UserState>().FirstOrDefault();
                if (userState == null)
                {
                    throw new InvalidOperationException("UserState must be defined and added before adding user-scoped state accessors.");
                }

                // Create the custom state accessor.
                // State accessors enable other components to read and write individual properties of state.
                var accessors = new WeatherBotAccessors(conversationState, userState)
                {
                    ConversationDialogState = conversationState.CreateProperty<DialogState>("DialogState"),
                    BotWeatherQuery = userState.CreateProperty<BotWeatherQueryDto>("BotWeatherQuery"),
                };

                return accessors;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework();
        }
    }
}
