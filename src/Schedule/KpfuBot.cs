using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vk.Bot.Framework;
using VkNet.Model.GroupUpdate;

namespace Schedule
{
    public class KpfuBot : BotBase<KpfuBot>
    {
        private ILogger<KpfuBot> _logger;

        public KpfuBot(ILogger<KpfuBot> logger)
        {
            _logger = logger;
        }

        public override Task HandleUnknownUpdate(GroupUpdate update)
        {
            _logger.LogWarning($"Unknown update: type:{update.Type} text:{update?.Message?.Text}");
            return Task.CompletedTask;
        }

        public override Task HandleFaultedUpdate(GroupUpdate update, Exception e)
        {
            _logger.LogError($"Failed update: type:{update.Type} text:{update?.Message?.Text} {JsonConvert.SerializeObject(e)}");
            return Task.CompletedTask;
        }
    }
}