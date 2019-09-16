using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vk.Bot.Framework.Abstractions;
using VkNet.Abstractions;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.GroupUpdate;

namespace Vk.Bot.Framework
{
    /// <summary>
    /// Manages bot and sends updates to handlers
    /// </summary>
    /// <typeparam name="TBot">Type of bot</typeparam>
    public class BotManager<TBot> : IBotManager<TBot>
        where TBot : BotBase<TBot>
    {
        /// <summary>
        /// Instance of bot under management
        /// </summary>
        public IBot Bot => _bot;

        /// <summary>
        /// Gets webhook's url from bot options provided
        /// </summary>
        public string WebhookUrl { get;}

        private readonly TBot _bot;

        private readonly IUpdateParser<TBot> _updateParser;

        private readonly ILogger<BotManager<TBot>> _logger;

        private readonly VkOptions<TBot> _botOptions;


        /// <summary>
        /// Initializes a new Bot Manager
        /// </summary>
        /// <param name="bot">Bot to be managed</param>
        /// <param name="updateParser">List of update parsers for the bot</param>
        /// <param name="botOptions">Options used to configure the bot</param>
        /// <param name="logger"></param>
        public BotManager(TBot bot, IVkApi vkApi, IUpdateParser<TBot> updateParser, IOptions<VkOptions<TBot>> botOptions, ILogger<BotManager<TBot>> logger)
        {
            _bot = bot;
            _bot.VkApiClient = vkApi;
            _updateParser = updateParser;
            _logger = logger;
            _botOptions = botOptions.Value;
            logger?.LogInformation("Bot logging started");
            WebhookUrl = _botOptions.WebhookUrl;
        }

        /// <summary>
        /// Handle the update
        /// </summary>
        /// <param name="update">Update to be handled</param>
        /// <returns></returns>
        public async Task HandleUpdateAsync(GroupUpdate update)
        {
            if (update.Message != null && update.Type == GroupUpdateType.MessageNew)
            {
                _logger?.LogInformation("Incoming update: {0}",
                    $"chatId: {update.Message?.FromId} // message: {update.Message?.Text}");
                try
                {
                    var handlers = _updateParser.FindHandlersForUpdate(_bot, update).ToList();

                    if (!handlers.Any())
                    {
                        await _bot.HandleUnknownUpdate(update);
                    }

                    foreach (var handler in handlers)
                    {
                        var result = await handler.HandleUpdateAsync(_bot, update);
                        if (result == UpdateHandlingResult.Handled)
                        {
                            return;
                        }
                    }

                }
                catch (Exception e)
                {
                    await _bot.HandleFaultedUpdate(update, e);
                }
            }
        }
    }
}