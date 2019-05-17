using Vk.Bot.Framework.Abstractions;

namespace Schedule.Commands
{
    public class DefaultCommandArgs : ICommandArgs
    {
        public string RawInput { get; set; }

        public string ArgsInput { get; set; }
    }
}