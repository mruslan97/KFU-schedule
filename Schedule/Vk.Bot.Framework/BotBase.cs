using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Vk.Bot.Framework.Abstractions;
using VkNet;
using VkNet.Abstractions;
using VkNet.Model.GroupUpdate;

namespace Vk.Bot.Framework
{
    /// <summary>
    /// Base class for implementing Bots
    /// </summary>
    /// <typeparam name="TBot">Type of Bot</typeparam>
    public abstract class BotBase<TBot> : IBot
        where TBot : class, IBot
    {
        public IVkApi VkApiClient { get; set; }

        /// <summary>
        /// Options used to the configure the bot instance
        /// </summary>
        public VkOptions<TBot> BotOptions { get; set; }

        /// <summary>
        /// Responsible for handling bot updates that don't have any handler
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        public abstract Task HandleUnknownUpdate(GroupUpdate update);

        /// <summary>
        /// Receives the update when the hanlding process throws an exception for the update
        /// </summary>
        /// <param name="update"></param>
        /// <param name="e">Exception thrown while processing the update</param>
        /// <returns></returns>
        public abstract Task HandleFaultedUpdate(GroupUpdate update, Exception e);
    }
}