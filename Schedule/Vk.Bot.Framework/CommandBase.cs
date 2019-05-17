using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vk.Bot.Framework.Abstractions;
using VkNet.Enums;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.GroupUpdate;

namespace Vk.Bot.Framework
{
    /// <summary>
    /// Base class for the bot commands
    /// </summary>
    /// <typeparam name="TCommandArgs">Type of the command argument this command accepts</typeparam>
    public abstract class CommandBase<TCommandArgs> : ICommand<TCommandArgs>
        where TCommandArgs : ICommandArgs, new()
    {
        /// <summary>
        /// Command name without leading '/'
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Instance of the bot this command is operating for
        /// </summary>
        protected IBot Bot { get; set; }

        public ILogger<CommandBase<TCommandArgs>> Logger { get; set; }

        /// <summary>
        /// Initializes a new bot command with specified command name
        /// </summary>
        /// <param name="name">This command's name without leading '/'</param>
        protected CommandBase(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Indicates whether this handler should receive the update for handling by quickly checking the update type such as text, photo, or etc.
        /// </summary>
        /// <param name="bot">Instance of the bot this command is operating for</param>
        /// <param name="update">Update for the bot</param>
        /// <returns><value>true</value> if this handler should get the update; otherwise <value>false</value></returns>
        public virtual bool CanHandleUpdate(IBot bot, GroupUpdate update)
        {
           
                Bot = Bot ?? bot;

                var isTextMessage = update.Type == GroupUpdateType.MessageNew;

                return isTextMessage && CanHandleCommand(update);
            
        }

        /// <summary>
        /// Handles the update for bot. This method will be called only if CanHandleUpdate returns <value>true</value>
        /// </summary>
        /// <param name="bot">Instance of the bot this command is operating for</param>
        /// <param name="update">The update to be handled</param>
        /// <returns>Result of handling this update</returns>
        public virtual async Task<UpdateHandlingResult> HandleUpdateAsync(IBot bot, GroupUpdate update)
        {
            Bot = Bot ?? bot;
            return await HandleCommand(update);
        }

        /// <summary>
        /// Indicates whether this command wants to handle the update by quickly checking the update type such as text, photo, or etc.
        /// </summary>
        /// <param name="update">The update to be handled</param>
        /// <returns><value>true</value> if this command should handle the update; otherwise <value>false</value></returns>
        protected virtual bool CanHandleCommand(GroupUpdate update)
        {

            var canHandle = false;
            try
            {
                if (update.Message.Text.Equals(Name))
                    canHandle = true;
            }
            catch (Exception e)
            {
                Logger.LogError($"failed to handle message {JsonConvert.SerializeObject(update)}, ex: {JsonConvert.SerializeObject(e)}");
            }

            return canHandle;
        }

        /// <summary>
        /// Handle the command update
        /// </summary>
        /// <param name="update">Command update to be handled</param>
        /// <returns>Result of handling this update</returns>
        public abstract Task<UpdateHandlingResult> HandleCommand(GroupUpdate update);
    }
}