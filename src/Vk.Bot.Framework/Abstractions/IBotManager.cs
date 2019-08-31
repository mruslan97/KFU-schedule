using System.Threading.Tasks;
using VkNet.Model.GroupUpdate;

namespace Vk.Bot.Framework.Abstractions
{
    /// <summary>
    /// Менеджер бота, отвечает за рассылку обновлений хендлерам(командам)
    /// </summary>
    /// <typeparam name="TBot">Бот</typeparam>
    public interface IBotManager<TBot>
    where TBot : class, IBot
    {
        /// <summary>
        /// Gets webhook's url from bot options provided
        /// </summary>
        string WebhookUrl { get; }

        Task HandleUpdateAsync(GroupUpdate update);
    }
}