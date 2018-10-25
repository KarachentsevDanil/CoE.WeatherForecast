using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using WeatherProvider.Bot.Model;

namespace WeatherProvider.Bot.State
{
    public class WeatherBotAccessors
    {
        public WeatherBotAccessors(ConversationState conversationState, UserState userState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
        }

        public IStatePropertyAccessor<DialogState> ConversationDialogState { get; set; }
        
        public IStatePropertyAccessor<BotWeatherQueryDto> BotWeatherQuery { get; set; }
        
        public ConversationState ConversationState { get; }
        
        public UserState UserState { get; }
    }
}
