using System.Collections.Generic;
using System.Threading.Tasks;
using Schedule.Entities;

namespace Schedule.Services
{
    /// <summary>
    /// Provide data from official schedule source
    /// </summary>
    public interface IParserService
    {
        /// <summary>
        /// Get actual groups list
        /// </summary>
        /// <returns></returns>
        Task<List<Group>> GetGroups();
        
        /// <summary>
        /// Get actual teachers list
        /// </summary>
        /// <returns></returns>
        Task<List<Teacher>> GetTeachers();

        /// <summary>
        /// Get actual subjects list
        /// </summary>
        /// <returns></returns>
        Task<List<Subject>> GetSubjects(IEnumerable<Group> groups);
    }
}