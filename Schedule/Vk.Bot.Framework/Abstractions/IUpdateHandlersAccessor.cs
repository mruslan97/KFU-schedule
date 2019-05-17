using System.Collections.Generic;

namespace Vk.Bot.Framework.Abstractions
{
    /// <summary>
    /// Provides a list of update handlers for the bot
    /// </summary>
    public interface IUpdateHandlersAccessor<TBot>
        where TBot : class, IBot
    {
        /// <summary>
        /// Gets a list of update handlers for the bot
        /// </summary>
        IEnumerable<IUpdateHandler> UpdateHandlers { get; }
    }
}