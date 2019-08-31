using System;
using System.Threading.Tasks;
using Vk.Bot.Framework;
using Vk.Bot.Framework.Abstractions;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.GroupUpdate;
using VkNet.Model.RequestParams;

namespace Schedule.Commands
{
    public class HelpCommand : CommandBase<DefaultCommandArgs>
    {
        public HelpCommand() : base(name: "старт")
        {
        }

        public override bool CanHandleUpdate(IBot bot, GroupUpdate update)
        {
            Console.WriteLine("Перехват апдейта из команды)");
            return update.Type == GroupUpdateType.MessageReply || update.Type == GroupUpdateType.MessageNew;
        }

        public override async Task<UpdateHandlingResult> HandleCommand(GroupUpdate update)
        {
            if (update.Type == GroupUpdateType.MessageReply)
                return UpdateHandlingResult.Handled;
            var random = new Random();
            await Task.FromResult(Bot.VkApiClient.Messages.Send(new MessagesSendParams
            {
                UserId = update.Message.FromId,
                Message = "Test message",
                PeerId = 181963334,
                RandomId = random.Next(int.MaxValue)
            }));

            return UpdateHandlingResult.Handled;
        }
    }
}