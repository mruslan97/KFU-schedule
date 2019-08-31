using System.Collections.Generic;
using Vk.Bot.Framework.Abstractions;

namespace Vk.Bot.Framework
{
    /// <summary>
    /// Provides a list of update handlers for the bot
    /// </summary>
    /// <typeparam name="TBot">Type of bot</typeparam>
    public class UpdateHandlersAccessor<TBot> : IUpdateHandlersAccessor<TBot>
        where TBot : class, IBot
    {
        /// <summary>
        /// Gets a list of update handlers for the bot
        /// </summary>
        public IEnumerable<IUpdateHandler> UpdateHandlers { get; }

        /// <summary>
        /// Initializes the accessor with a list of update handlers
        /// </summary>
        /// <param name="handlers">List of update handlers for the bot</param>
        public UpdateHandlersAccessor(IEnumerable<IUpdateHandler> handlers)
        {
            UpdateHandlers = handlers;
        }
    }
}