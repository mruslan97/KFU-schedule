using System.Threading.Tasks;

namespace Schedule.Services
{
    /// <summary>
    /// Сервис которые отправляет шаблонные сообщения
    /// </summary>
    public interface IVkSenderService
    {
        /// <summary>
        /// Отправить уведомление об ошибке
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task SendError(long userId);
    }
}