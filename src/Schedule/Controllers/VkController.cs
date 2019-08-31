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
    //[ApiController, Route("api/[controller]")]
    public class VkController : Controller
    {
        public IVkApi VkApi { get; set; }

        public ILogger<VkController> Logger { get; set; }

        [HttpPost]
        public async Task<IActionResult> ReceiveMessage([FromBody] JToken body)
        {
            try
            {
                var vkResponse = new VkResponse(body);
                var groupUpdate = GroupUpdate.FromJson(vkResponse);
                var random = new Random();
                if (groupUpdate.Type == GroupUpdateType.MessageNew)
                {
                    VkApi.Messages.SetActivity(groupUpdate.Message.FromId.ToString(), MessageActivityType.Typing, 28691895, 181963334);
                    await Task.Delay(8000);
                    Logger.LogInformation($"New message from {groupUpdate.Message.FromId} {groupUpdate.Message.Text}");

                    VkApi.Messages.Send(new MessagesSendParams
                    {
                        UserId = groupUpdate.Message.FromId,
                        Message = "Test message",
                        PeerId = 181963334,
                        RandomId = random.Next(int.MaxValue)
                    });

                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                return Ok("ok");
            }

            return Ok("ok");
        }
    }
}