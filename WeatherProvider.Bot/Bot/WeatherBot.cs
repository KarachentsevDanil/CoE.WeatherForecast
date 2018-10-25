using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using WeatherForecast.Provider.Api;
using WeatherForecast.Provider.Dto;
using WeatherProvider.Bot.Constants;
using WeatherProvider.Bot.Model;
using WeatherProvider.Bot.State;

namespace WeatherProvider.Bot.Bot
{
    public class WeatherBot : IBot
    {
        private readonly IWeatherForecastApi _weatherForecastApi;
        private readonly WeatherBotAccessors _accessors;
        private readonly DialogSet _dialogs;
        private readonly LuisRecognizer _luisRecognizer;

        public WeatherBot(WeatherBotAccessors accessors, IWeatherForecastApi weatherForecastApi, LuisRecognizer luisRecognizer)
        {
            _accessors = accessors;
            _weatherForecastApi = weatherForecastApi;
            _luisRecognizer = luisRecognizer;

            _dialogs = new DialogSet(accessors.ConversationDialogState);

            var waterfallSteps = new WaterfallStep[]
            {
                //CityStepAsync,
                //StoreCityNameStepAsync,
                SelectUnitsStepAsync,
                DisplayWeatherForecastStepAsync
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            _dialogs.Add(new WaterfallDialog(BotConstants.DialogName, waterfallSteps));
            _dialogs.Add(new TextPrompt(BotConstants.CityPromptName));
            _dialogs.Add(new ChoicePrompt(BotConstants.UnitsPromptName));
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = new CancellationToken())
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);

                if (dialogContext.ActiveDialog == null)
                {
                    var result = await _luisRecognizer.RecognizeAsync(turnContext, cancellationToken);
                    var topIntent = result?.GetTopScoringIntent();

                    if (topIntent.HasValue && topIntent.Value.intent == BotConstants.WeatherForecastIntentName)
                    {
                        var city = result.Entities["City"]?.FirstOrDefault()?.ToString();

                        if (!string.IsNullOrEmpty(city))
                        {
                            var results = await dialogContext.ContinueDialogAsync(cancellationToken);

                            var weatherQuery = await _accessors.BotWeatherQuery.GetAsync(turnContext, () => new BotWeatherQueryDto(), cancellationToken);
                            weatherQuery.City = city;

                            if (results.Status == DialogTurnStatus.Empty)
                            {
                                await dialogContext.BeginDialogAsync(BotConstants.DialogName, null, cancellationToken);
                            }
                        }
                    }
                }
                else
                {
                    var results = await dialogContext.ContinueDialogAsync(cancellationToken);

                    if (results.Status == DialogTurnStatus.Empty)
                    {
                        await dialogContext.BeginDialogAsync(BotConstants.DialogName, null, cancellationToken);
                    }
                }
            }
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (turnContext.Activity.MembersAdded != null)
                {
                    await SendWelcomeMessageAsync(turnContext, cancellationToken);
                }
            }
            else
            {
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} event detected", cancellationToken: cancellationToken);
            }

            await _accessors.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);

            await _accessors.UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        #region Dialog Handlers

        private async Task<DialogTurnResult> CityStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(BotConstants.CityPromptName,
                new PromptOptions { Prompt = MessageFactory.Text(BotConstants.EnterCityNameText) }, cancellationToken);
        }

        private async Task<DialogTurnResult> StoreCityNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var weatherQuery = await _accessors.BotWeatherQuery.GetAsync(stepContext.Context, () => new BotWeatherQueryDto(), cancellationToken);
            weatherQuery.City = (string)stepContext.Result;
            return await SelectUnitsStepAsync(stepContext, cancellationToken);
        }

        private static async Task<DialogTurnResult> SelectUnitsStepAsync(WaterfallStepContext step, CancellationToken cancellationToken)
        {
            return await step.PromptAsync(BotConstants.UnitsPromptName, GenerateUnits(step.Context.Activity), cancellationToken);
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

        private async Task<DialogTurnResult> DisplayWeatherForecastStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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

        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var reply = turnContext.Activity.CreateReply();
                    reply.Text = BotConstants.WelcomeText;
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                }
            }
        }

        private string DisplayWeatherForecast(WeatherForecastDto dto, string temperatureUnit)
        {
            return $"{dto.Place}, {dto.Country}." +
                   $" {dto.Temperature} {temperatureUnit} temperature from {dto.MinTemperature} {temperatureUnit} to {dto.MaxTemperature} {temperatureUnit}," +
                   $" {dto.WindSpeed} m/s, {dto.Pressure} hPa, humidity: {dto.Humidity}%, overcast: {dto.Overcast}%";
        }
        #endregion
    }
}
