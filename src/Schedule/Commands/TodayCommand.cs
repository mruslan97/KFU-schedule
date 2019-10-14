using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Schedule.Entities;
using Schedule.Entities.Enums;
using Schedule.Extensions;
using Schedule.Services;
using Storage.Abstractions.UnitOfWork;
using Vk.Bot.Framework;
using Vk.Bot.Framework.Abstractions;
using VkNet.Abstractions;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Attachments;
using VkNet.Model.GroupUpdate;
using VkNet.Model.RequestParams;

namespace Schedule.Commands
{
    public class TodayCommand : CommandBase<DefaultCommandArgs>
    {
        private ITimespanRepository<VkUser> _users;

        private ITimespanRepository<Subject> _subjects;

        /// <summary> паттерн uow </summary>
        private IUnitOfWorkFactory _uowFactory;

        private IVkApi _vkApi;

        private IOptions<VkOptions<KpfuBot>> _options;

        private IObjectStorageService _storageService;

        private IVkSenderService _vkSenderService;

        private ILogger<TodayCommand> _logger;

        public TodayCommand(ITimespanRepository<Subject> subjects,
            ITimespanRepository<VkUser> users,
            IUnitOfWorkFactory uowFactory,
            IVkApi vkApi, IOptions<VkOptions<KpfuBot>> options,
            IObjectStorageService storageService,
            IVkSenderService vkSenderService,
            ILogger<TodayCommand> logger) : base("На сегодня")
        {
            _subjects = subjects;
            _users = users;
            _uowFactory = uowFactory;
            _vkApi = vkApi;
            _options = options;
            _storageService = storageService;
            _vkSenderService = vkSenderService;
            _logger = logger;
        }

        public override bool CanHandleUpdate(IBot bot, GroupUpdate update)
        {
            return update.Message != null && update.Message.Text.ToLower().Contains("сегодня");
        }

        public override async Task<UpdateHandlingResult> HandleCommand(GroupUpdate update)
        {
            try
            {
                var user = _users.GetAll().Include(x => x.Group)
                    .FirstOrDefault(x => x.UserId == update.Message.FromId);
                var random = new Random();

                if (user == null)
                {
                    _vkApi.Messages.Send(new MessagesSendParams
                    {
                        UserId = update.Message.FromId,
                        Message = $"Для начала работы введи команду старт или начать",
                        PeerId = _options.Value.GroupId,
                        RandomId = random.Next(int.MaxValue)
                    });
                    return await Task.FromResult(UpdateHandlingResult.Handled);
                }
                
                _vkApi.Messages.SetActivity(update.Message.FromId.ToString(), 
                    MessageActivityType.Typing, 
                    update.Message.FromId, 
                    (ulong)_options.Value.GroupId );

                var dayOfWeek = DateTime.Now.DayOfWeek == 0 ? 1 : (int) DateTime.Now.DayOfWeek;

                if (user.ScheduleType == ScheduleType.Text)
                {
                    var subjects = _subjects.GetAll().Where(x => x.GroupId == user.GroupId
                                                                 && x.DayOfWeek == dayOfWeek
                                                                 && x.StartDay.Value <= DateTime.Today
                                                                 && x.EndDay.Value >= DateTime.Today).ToList();
                    _vkApi.Messages.Send(new MessagesSendParams
                    {
                        UserId = user.UserId,
                        Message = subjects.ToMessage((DayOfWeek) dayOfWeek),
                        PeerId = _options.Value.GroupId,
                        RandomId = random.Next(int.MaxValue),
                    });
                }
                else
                {
                    var uploadServer = _vkApi.Photo.GetMessagesUploadServer(user.UserId);
                    var image = await _storageService.GetDay(user.Group.GroupName, dayOfWeek);
                    var response = await _vkSenderService.UploadImage(uploadServer.UploadUrl, image.ToArray());
                    var photo = _vkApi.Photo.SaveMessagesPhoto(response).SingleOrDefault();

                    _vkApi.Messages.Send(new MessagesSendParams
                    {
                        UserId = update.Message.FromId,
                        PeerId = _options.Value.GroupId,
                        Attachments = new List<MediaAttachment> {photo},
                        RandomId = random.Next(int.MaxValue)
                    });
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Произошла ошибка в {nameof(HandleCommand)}, {JsonConvert.SerializeObject(e)}");
                await _vkSenderService.SendError((long) update.Message.UserId);
                return UpdateHandlingResult.Handled;
            }

            return await Task.FromResult(UpdateHandlingResult.Handled);
        }
    }
}