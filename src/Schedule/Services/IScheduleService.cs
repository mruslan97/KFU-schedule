using System.Collections.Generic;
using System.Threading.Tasks;
using Schedule.Entities;

namespace Schedule.Services
{
    /// <summary>
    /// schedule service, which include methods for initialize and update local schedule storage
    /// </summary>
    public interface IScheduleService
    {
        /// <summary>
        /// Initialize local base with actual schedule(inc. teachers, groups, subjects)
        /// </summary>
        /// <returns></returns>
        Task InitializeLocalStorage(); 

        /// <summary> Update local storage(subjects for actual groups) </summary>
        Task UpdateSubjects();

        /// <summary>
        /// Update local storage for new semestr(merge with actual data)
        /// </summary>
        /// <returns></returns>
        Task UpdateLocalDb();
    }
}