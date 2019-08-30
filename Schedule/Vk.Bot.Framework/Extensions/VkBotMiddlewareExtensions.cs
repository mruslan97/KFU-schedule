using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Vk.Bot.Framework.Abstractions;
using Vk.Bot.Framework.Middlewares;

namespace Vk.Bot.Framework.Extensions
{
    public static class VkBotMiddlewareExtensions
    {
        /// <summary>
        /// Add Telegram bot webhook handling functionality to the pipeline
        /// </summary>
        /// <typeparam name="TBot">Type of bot</typeparam>
        /// <param name="app">Instance of IApplicationBuilder</param>
        /// <returns>Instance of IApplicationBuilder</returns>
        public static IApplicationBuilder UseVkBot<TBot>(this IApplicationBuilder app)
            where TBot : BotBase<TBot>
        {
            return app.UseMiddleware<BotMiddleware<TBot>>();
        }

        private static IBotManager<TBot> FindBotManager<TBot>(IApplicationBuilder app)
            where TBot : BotBase<TBot>
        {
            IBotManager<TBot> botManager;
            try
            {
                botManager = app.ApplicationServices.GetRequiredService<IBotManager<TBot>>();
                if (botManager == null)
                {
                    throw new NullReferenceException();
                }
            }
            catch (Exception)
            {
                throw new ConfigurationException(
                    "Bot Manager service is not available", string.Format("Use services.{0}<{1}>()",
                        nameof(VkBotFrameworkIServiceCollectionExtensions.AddVkBot), typeof(TBot).Name));
            }
            return botManager;
        }
    }
}