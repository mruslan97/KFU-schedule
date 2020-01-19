using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentDateTime;
using Microsoft.Extensions.Options;
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
    public class WeekCommand : CommandBase<DefaultCommandArgs>
    {
        private ITimespanRepository<VkUser> _users;

        private ITimespanRepository<Subject> _subjects;

        /// <summary> паттерн uow </summary>
        private IUnitOfWorkFactory _uowFactory;

        private IVkApi _vkApi;

        private IOptions<VkOptions<KpfuBot>> _options;
        
        private IObjectStorageService _storageService;

        private IVkSenderService _vkSenderService;

        public WeekCommand(ITimespanRepository<Subject> subjects,
            ITimespanRepository<VkUser> users, 
            IUnitOfWorkFactory uowFactory, 
            IVkApi vkApi, 
            IOptions<VkOptions<KpfuBot>> options, 
            IObjectStorageService storageService, 
            IVkSenderService vkSenderService) : base("На неделю")
        {
            _subjects = subjects;
            _users = users;
            _uowFactory = uowFactory;
            _vkApi = vkApi;
            _options = options;
            _storageService = storageService;
            _vkSenderService = vkSenderService;
        }

        public override bool CanHandleUpdate(IBot bot, GroupUpdate update)
        {
            return update.Message != null && update.Message.Text.ToLower().Contains("неделю");
        }

        public override Task<UpdateHandlingResult> HandleCommand(GroupUpdate update)
        {
            var user = _users.GetAll().FirstOrDefault(x => x.UserId == update.Message.FromId);
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
                return Task.FromResult(UpdateHandlingResult.Handled);
            }

            if (user.ScheduleType == ScheduleType.Text)
            {

                var monday = DateTime.Now.Previous(DayOfWeek.Monday);
                var sunday = DateTime.Now.Previous(DayOfWeek.Sunday);
                var subjects = _subjects.GetAll().Where(x => x.GroupId == user.GroupId
                    //&& x.StartDay.Value <= monday
                ).ToList();//&& x.EndDay.Value >= sunday).ToList(); TODO uncomment 11.02 
                var daysOfWeek = subjects.OrderBy(x => x.DayOfWeek).Select(x => x.DayOfWeek).Distinct();
                foreach (var day in daysOfWeek)
                {
                    _vkApi.Messages.Send(new MessagesSendParams
                    {
                        UserId = user.UserId,
                        Message = subjects.Where(x => x.DayOfWeek == day).ToMessage((DayOfWeek) day),
                        PeerId = _options.Value.GroupId,
                        RandomId = random.Next(int.MaxValue),
                    });
                }
            }
            else
            {
                var uploadServer = _vkApi.Photo.GetMessagesUploadServer(user.UserId);
                var images = new List<MediaAttachment>();
           
                // Отправляем в background для избежания дублей, потому что вк требует быстрого ответа.
                Task.Run(async () =>
                {
                    _vkApi.Messages.SetActivity(update.Message.FromId.ToString(), 
                        MessageActivityType.Typing, 
                        update.Message.FromId, 
                        (ulong)_options.Value.GroupId );
                
                    for (var i = 1; i <= 6; i++)
                    {
                        var image = await _storageService.GetDay(user.Group.GroupName, i);

                        var response = await _vkSenderService.UploadImage(uploadServer.UploadUrl, image.ToArray());
                        var photo = _vkApi.Photo.SaveMessagesPhoto(response).SingleOrDefault();
                        images.Add(photo);

                    }

                    _vkApi.Messages.Send(new MessagesSendParams
                    {
                        UserId = update.Message.FromId,
                        PeerId = _options.Value.GroupId,
                        Attachments = images,
                        RandomId = random.Next(int.MaxValue)
                    });
                });
            }


            return Task.FromResult(UpdateHandlingResult.Handled);
        }
    }
}