using VkNet.Abstractions;

namespace Vk.Bot.Framework.Abstractions
{
    public interface IBot
    {
        /// <summary> Инстанс клиента для вк api </summary>
        IVkApi VkApiClient { get; }
    }
}