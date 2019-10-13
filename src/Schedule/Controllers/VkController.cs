using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using VkNet.Abstractions;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.GroupUpdate;
using VkNet.Model.RequestParams;
using VkNet.Utils;

namespace Schedule.Controllers
{
    /// <summary>
    /// Контроллер для подтверждения в группе ВК
    /// </summary>
    //[ApiController, Route("api/[controller]")]
    public class VkController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> ReceiveMessage([FromBody] JToken body)
        {

            return Ok("");
        }
    }
}