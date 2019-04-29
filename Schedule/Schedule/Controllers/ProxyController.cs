using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Schedule.Entities;
using Schedule.Entities.Kpfu;

namespace Schedule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProxyController : Controller
    {
        public IOptions<Models.DomainOptions> Options { get; set; }

        // GET
        [HttpGet("subjects")]
        public async Task<IEnumerable<KpfuSubject>> GetSubjects(string group)
        {
            using (var httpClient = new HttpClient())
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var encoding = CodePagesEncodingProvider.Instance.GetEncoding(1251);
                var response = await httpClient.GetAsync(
                    $"{Options.Value.KpfuHost}/e-ksu/portal_pg_mobile.get_schedule?p_name_group={group}" +
                    $"&p_stud_year={Options.Value.Year}&p_stud_semester={Options.Value.Semester}");
                var json = await response.Content.ReadAsStringAsync();
                var subjectRoot = JsonConvert.DeserializeObject<KpfuSubjectRoot>(json);
                
                return subjectRoot.Subjects;
            }

        }
    }
}