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
using WeatherProvider.Bot.Dialogs;
using WeatherProvider.Bot.Model;
using WeatherProvider.Bot.State;

namespace WeatherProvider.Bot.Bot
{
    public class WeatherBot : IBot
    {
        private readonly DialogSet _dialogs;
        private readonly LuisRecognizer _luisRecognizer;
        private readonly IWeatherDialog _weatherDialog;
        private readonly WeatherBotAccessors _accessors;

        public WeatherBot(WeatherBotAccessors accessors, LuisRecognizer luisRecognizer, IWeatherDialog weatherDialog)
        {
            _accessors = accessors;
            _luisRecognizer = luisRecognizer;
            _weatherDialog = weatherDialog;

            _dialogs = new DialogSet(accessors.ConversationDialogState);

            var waterfallSteps = new WaterfallStep[]
            {
                _weatherDialog.SelectUnitsStepAsync,
                _weatherDialog.DisplayWeatherForecastStepAsync
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
                    await RecognizeUserIntent(turnContext, cancellationToken, dialogContext);
                }
                else
                {
                    await BeginWeatherDialog(dialogContext, cancellationToken);
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

        private async Task RecognizeUserIntent(ITurnContext turnContext, CancellationToken cancellationToken, DialogContext dialogContext)
        {
            var result = await _luisRecognizer.RecognizeAsync(turnContext, cancellationToken);
            var topIntent = result?.GetTopScoringIntent();

            if (topIntent.HasValue && topIntent.Value.intent == BotConstants.WeatherForecastIntentName)
            {
                var city = result.Entities["City"]?.FirstOrDefault()?.ToString();

                if (!string.IsNullOrEmpty(city))
                {
                    var weatherQuery = await _accessors.BotWeatherQuery.GetAsync(turnContext, () => new BotWeatherQueryDto(), cancellationToken);
                    weatherQuery.City = city;

                    await BeginWeatherDialog(dialogContext, cancellationToken);
                }
            }
        }

        private static async Task BeginWeatherDialog(DialogContext dialogContext, CancellationToken cancellationToken)
        {
            var results = await dialogContext.ContinueDialogAsync(cancellationToken);

            if (results.Status == DialogTurnStatus.Empty)
            {
                await dialogContext.BeginDialogAsync(BotConstants.DialogName, null, cancellationToken);
            }
        }
        
        #region Dialog Handlers


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

        #endregion
    }
}
