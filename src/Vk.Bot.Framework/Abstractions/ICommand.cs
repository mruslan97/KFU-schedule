﻿using System.Threading.Tasks;
using VkNet.Model.GroupUpdate;

namespace Vk.Bot.Framework.Abstractions
{
    /// <summary>
    /// A Telegram bot command such as /start
    /// </summary>
    /// <typeparam name="TCommandArgs">Type of the command argument this command accepts</typeparam>
    public interface ICommand<in TCommandArgs> : IUpdateHandler
        where TCommandArgs : ICommandArgs
    {
        /// <summary>
        /// Gets this command's name without leading '/'
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Handle the command update
        /// </summary>
        /// <param name="update">Command update to be handled</param>
        /// <param name="args">Command arguments</param>
        /// <returns>Result of handling this update</returns>
        Task<UpdateHandlingResult> HandleCommand(GroupUpdate update);
    }
}