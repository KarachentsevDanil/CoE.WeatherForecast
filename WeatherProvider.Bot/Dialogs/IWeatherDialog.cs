using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace WeatherProvider.Bot.Dialogs
{
    public interface IWeatherDialog
    {
        Task<DialogTurnResult> SelectUnitsStepAsync(WaterfallStepContext step, CancellationToken cancellationToken);

        Task<DialogTurnResult> DisplayWeatherForecastStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken);
    }
}
