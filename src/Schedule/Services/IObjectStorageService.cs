using System.Collections.Generic;
using System.Threading.Tasks;
using CodeJam.Collections;

namespace Schedule.Services
{
    public interface IObjectStorageService
    {
        Task<IEnumerable<byte>> GetDay(string group, int day);
    }
}