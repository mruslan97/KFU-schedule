using System.Threading.Tasks;

namespace Schedule.Services
{
    /// <summary>
    /// Сервис обновления локальной базы
    /// </summary>
    public interface IUpdateService
    {
        /// <summary> Обновить группы  </summary>
        Task UpdateLocaleStorage();
    }
}