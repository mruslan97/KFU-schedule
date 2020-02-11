using System.Collections.Generic;
using System.Threading.Tasks;
using Schedule.Entities;

namespace Schedule.Services
{
    public interface IScheduleService
    {
        Task InitializeLocalStorage();

        Task<List<Group>> GetGroups();
    }
}