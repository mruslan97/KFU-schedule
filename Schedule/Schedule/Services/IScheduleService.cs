using System.Threading.Tasks;

namespace Schedule.Services
{
    public interface IScheduleService
    {
        Task InitializeLocalStorage();
    }
}