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

        /// <summary>
        /// Загрузить изображение на сервера вк
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<string> UploadImage(string url, byte[] data);
    }
}