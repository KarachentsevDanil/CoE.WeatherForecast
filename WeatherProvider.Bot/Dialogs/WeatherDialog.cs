using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using WeatherForecast.Provider.Api;
using WeatherForecast.Provider.Dto;
using WeatherProvider.Bot.Constants;
using WeatherProvider.Bot.Model;
using WeatherProvider.Bot.State;

namespace WeatherProvider.Bot.Dialogs
{
    public class WeatherDialog : IWeatherDialog
    {
        private readonly IWeatherForecastApi _weatherForecastApi;
        private readonly WeatherBotAccessors _accessors;

        public WeatherDialog(IWeatherForecastApi weatherForecastApi, WeatherBotAccessors accessors)
        {
            _weatherForecastApi = weatherForecastApi;
            _accessors = accessors;
        }

        public async Task<DialogTurnResult> SelectUnitsStepAsync(WaterfallStepContext step, CancellationToken cancellationToken)
        {
            return await step.PromptAsync(BotConstants.UnitsPromptName, GenerateUnits(step.Context.Activity), cancellationToken);
        }

        public async Task<DialogTurnResult> DisplayWeatherForecastStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Get the current profile object from user state.
            var botWeatherQuery = await _accessors.BotWeatherQuery.GetAsync(stepContext.Context, () => new BotWeatherQueryDto(), cancellationToken);
            botWeatherQuery.Units = stepContext.Context.Activity.Text;

            WeatherForecastDto weatherForecastDto;

            try
            {
                weatherForecastDto =
                    await _weatherForecastApi.GetWeatherForecast(botWeatherQuery.ToWeatherForecastDto());
            }
            catch (Exception)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Can't find the weather for {botWeatherQuery.City}. Please try again."), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(DisplayWeatherForecast(weatherForecastDto, botWeatherQuery.GetDisplayUnitsValue())), cancellationToken);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(BotConstants.RestartBotTest), cancellationToken);

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog, here it is the end.
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        private static PromptOptions GenerateUnits(Activity activity)
        {
            // Create options for the prompt
            var options = new PromptOptions
            {
                Prompt = activity.CreateReply(BotConstants.SelectTemperatureUnitsText),
                Choices = new List<Choice>
                {
                    new Choice { Value = "Celsius" },
                    new Choice { Value = "Fahrenheit" },
                    new Choice { Value = "Kelvin" }
                },
            };

            return options;
        }

        private string DisplayWeatherForecast(WeatherForecastDto dto, string temperatureUnit)
        {
            return $"{dto.Place}, {dto.Country}." +
                   $" {dto.Temperature} {temperatureUnit} temperature from {dto.MinTemperature} {temperatureUnit} to {dto.MaxTemperature} {temperatureUnit}," +
                   $" {dto.WindSpeed} m/s, {dto.Pressure} hPa, humidity: {dto.Humidity}%, overcast: {dto.Overcast}%";
        }
    }
}
