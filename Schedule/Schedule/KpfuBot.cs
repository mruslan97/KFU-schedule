using System;
using System.Threading.Tasks;
using Vk.Bot.Framework;
using VkNet.Model.GroupUpdate;

namespace Schedule
{
    public class KpfuBot : BotBase<KpfuBot>
    {
        public override Task HandleUnknownUpdate(GroupUpdate update)
        {
            throw new NotImplementedException();
        }

        public override Task HandleFaultedUpdate(GroupUpdate update, Exception e)
        {
            throw new NotImplementedException();
        }
    }
}