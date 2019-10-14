using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using Vk.Bot.Framework;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Schedule.Services.Impl
{
    /// <inheritdoc />
    public class VkSenderService : IVkSenderService
    {
        /// <summary> Клиент для вк </summary>
        public IVkApi VkApi { get; set; }

        /// <summary>
        /// options
        /// </summary>
        public IOptions<VkOptions<KpfuBot>> Options { get; set; }
        
        public ILogger<VkSenderService> Logger { get; set; }
        
        public IHttpClientFactory HttpClientFactory { get; set; }
        
        public Task SendError(long userId)
        {
            try
            {
                var random = new Random();
                VkApi.Messages.Send(new MessagesSendParams
                {
                    UserId = userId,
                    Message = $"К сожалению, произошла ошибка на сервере :(",
                    PeerId = Options.Value.GroupId,
                    RandomId = random.Next(int.MaxValue)
                });
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                Logger.LogError($"Ошибка при отправке error сообщения {JsonConvert.SerializeObject(e)}");
                return Task.CompletedTask;
            }
        }

        public async Task<string> UploadImage(string url, byte[] data)
        {
            using (var client = HttpClientFactory.CreateClient())
            {
                var requestContent = new MultipartFormDataContent();
                var imageContent = new ByteArrayContent(data);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                requestContent.Add(imageContent, "photo", "image.png");
                var encoding = CodePagesEncodingProvider.Instance.GetEncoding(1251);
                var response = await client.PostAsync(url, requestContent);
                var bytes = await response.Content.ReadAsByteArrayAsync();
                return encoding.GetString(bytes, 0, bytes.Length);
            }
        }
    }
}