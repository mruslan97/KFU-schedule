using Vk.Bot.Framework.Abstractions;

namespace Vk.Bot.Framework
{
    public class VkOptions<TBot>
        where TBot : class, IBot
    {
        public string ApiToken { get; set; }

        public string WebhookUrl { get; set; }

        public long GroupId { get; set; }
    }
}