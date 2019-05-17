﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vk.Bot.Framework.Abstractions;

namespace Vk.Bot.Framework.Extensions
{
    /// <summary>
    /// Extensoin methods for adding a Telegram Bot to an Microsoft.Extensions.DependencyInjection.IServiceCollection
    /// </summary>
    public static class VkBotFrameworkIServiceCollectionExtensions
    {
        private static IServiceCollection _services;

        /// <summary>
        /// Adds a Telegram bot to the service collection using the bot's options
        /// </summary>
        /// <typeparam name="TBot">Type of Telegarm bot</typeparam>
        /// <param name="services">Instance of IServiceCollection</param>
        /// <param name="botOptions">Optins for configuring the bot</param>
        /// <returns>Instance of bot framework builder</returns>
        public static IVkBotFrameworkBuilder<TBot> AddVkBot<TBot>
            (this IServiceCollection services, VkOptions<TBot> botOptions)
            where TBot : BotBase<TBot>
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (botOptions == null)
                throw new ArgumentNullException(nameof(botOptions));

            //ThrowExceptionIfAppSettingsInvalid<TBot>(botOptions.ApiToken, botOptions.BotUserName,
            //    botOptions.WebhookUrl, botOptions.PathToCertificate);

            _services = services;
            return new VkBotFrameworkBuilder<TBot>(botOptions);
        }

        /// <summary>
        /// Adds a Telegram bot to the service collection using configurations
        /// </summary>
        /// <typeparam name="TBot">Type of Telegarm bot</typeparam>
        /// <param name="services">Instance of IServiceCollection</param>
        /// <param name="config">Configuring for the bot</param>
        /// <returns>Instance of bot framework builder</returns>
        public static IVkBotFrameworkBuilder<TBot> AddVkBot<TBot>
            (this IServiceCollection services, IConfiguration config)
            where TBot : BotBase<TBot>
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (config == null)
                throw new ArgumentNullException(nameof(config));

            //ThrowExceptionIfAppSettingsInvalid<TBot>(
            //    config[nameof(BotOptions<TBot>.ApiToken)],
            //    config[nameof(BotOptions<TBot>.BotUserName)],
            //    config[nameof(BotOptions<TBot>.WebhookUrl)],
            //    config[nameof(BotOptions<TBot>.PathToCertificate)]);

            _services = services;
            return new VkBotFrameworkBuilder<TBot>(config);
        }

        /// <summary>
        /// Responsible for configuring services for the bot and adding them to the container
        /// </summary>
        /// <typeparam name="TBot">Type of bot</typeparam>
        public interface IVkBotFrameworkBuilder<TBot>
            where TBot : BotBase<TBot>
        {
            /// <summary>
            /// Configures an update handler for the bot
            /// </summary>
            /// <typeparam name="T">Type of update handler</typeparam>
            /// <returns>Itself</returns>
            IVkBotFrameworkBuilder<TBot> AddUpdateHandler<T>()
                where T : class, IUpdateHandler;

            /// <summary>
            /// Completes the configuration for the bot and adds all the services
            /// </summary>
            /// <returns>Instance of IServiceCollection</returns>
            IServiceCollection Configure();
        }

        private static void ThrowExceptionIfAppSettingsInvalid<TBot>(string apiToken, string botUserName,
            string webhookUrl = null,
            string certificatePath = null)
            where TBot : BotBase<TBot>
        {
            //if (string.IsNullOrWhiteSpace(apiToken))
            //    throw new ArgumentNullException(nameof(BotOptions<TBot>.ApiToken));

            //if (apiToken.Length < 25)
            //    throw new ConfigurationException($"API token `{apiToken}` is too short.", "Check bot's token with BotFather");

            //if (string.IsNullOrWhiteSpace(botUserName))
            //    throw new ArgumentNullException(nameof(BotOptions<TBot>.BotUserName));

            //if (!botUserName.EndsWith("bot", StringComparison.OrdinalIgnoreCase))
            //    throw new ConfigurationException($"Bot user name `{botUserName}` is not valid.",
            //        "Bot user names should end with `bot` or `_bot`.");

            //if (!string.IsNullOrWhiteSpace(webhookUrl) && !webhookUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            //    throw new ConfigurationException($"Webhook url `{webhookUrl}` is not a HTTPS url");

            //if (!string.IsNullOrWhiteSpace(certificatePath) && !File.Exists(certificatePath))
            //    throw new ConfigurationException($"Certificate file `{certificatePath}` does not exist.");

            // todo validate rest of the settings
        }

        /// <summary>
        /// Responsible for configuring services for the bot and adding them to the container
        /// </summary>
        /// <typeparam name="TBot">Type of bot</typeparam>
        public class VkBotFrameworkBuilder<TBot> : IVkBotFrameworkBuilder<TBot>
            where TBot : BotBase<TBot>
        {
            private readonly List<Type> _handlerTypes = new List<Type>();

            private readonly VkOptions<TBot> _botOptions;

            private readonly IConfiguration _configuration;

            /// <summary>
            /// Initializes and instance of this class with the options provided
            /// </summary>
            /// <param name="botOptions">Optoins for the bot</param>
            public VkBotFrameworkBuilder(VkOptions<TBot> botOptions)
            {
                _botOptions = botOptions;
            }

            /// <summary>
            /// Initializes and instance of this class with the configuration provided
            /// </summary>
            /// <param name="configuration">Configuration for the bot</param>
            public VkBotFrameworkBuilder(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            /// <summary>
            /// Configures an update handler for the bot
            /// </summary>
            /// <typeparam name="T">Type of update handler</typeparam>
            /// <returns>Itself</returns>
            public IVkBotFrameworkBuilder<TBot> AddUpdateHandler<T>()
                where T : class, IUpdateHandler
            {
                _handlerTypes.Add(typeof(T));
                return this;
            }

            /// <summary>
            /// Completes the configuration for the bot and adds all the services
            /// </summary>
            /// <returns>Instance of IServiceCollection</returns>
            public IServiceCollection Configure()
            {
                EnsureValidConfiguration();

                if (_botOptions != null)
                {
                    _services.Configure<VkOptions<TBot>>(x =>
                    {
                        x.ApiToken = _botOptions.ApiToken;
                        x.GroupId = _botOptions.GroupId;
                        x.WebhookUrl = _botOptions.WebhookUrl;
                    });
                }
                else
                {
                    _services.Configure<VkOptions<TBot>>(_configuration);
                }

                _services.AddScoped<TBot>();

                _handlerTypes.ForEach(x => _services.AddTransient(x));

                _services.AddScoped<IUpdateHandlersAccessor<TBot>>(factory =>
                {
                    var handlers = _handlerTypes.Select(x => (IUpdateHandler)factory.GetRequiredService(x)).ToArray();
                    return new UpdateHandlersAccessor<TBot>(handlers);
                });

                _services.AddScoped<IUpdateParser<TBot>, UpdateParser<TBot>>();

                _services.AddScoped<IBotManager<TBot>, BotManager<TBot>>();

                return _services;
            }

            private void EnsureValidConfiguration()
            {
                if (!_handlerTypes.Any())
                {
                    throw new ConfigurationException("No update handler is provided", $"Use {nameof(AddUpdateHandler)} method");
                }
                // ToDo: Validate others
            }
        }
    }
}