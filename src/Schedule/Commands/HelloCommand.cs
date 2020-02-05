using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Schedule.Entities;
using Schedule.Entities.Enums;
using Schedule.Extensions;
using Schedule.Models;
using Schedule.Services;
using Storage.Abstractions.UnitOfWork;
using Vk.Bot.Framework;
using Vk.Bot.Framework.Abstractions;
using VkNet.Abstractions;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.GroupUpdate;
using VkNet.Model.RequestParams;

namespace Schedule.Commands
{
    public class HelloCommand : CommandBase<DefaultCommandArgs>
    {
        private ITimespanRepository<VkUser> _users;

        private IVkSenderService _vkSenderService;

        /// <summary> паттерн uow </summary>
        private IUnitOfWorkFactory _uowFactory;

        private IVkApi _vkApi;

        private IOptions<VkOptions<KpfuBot>> _options;

        public HelloCommand(IVkApi vkApi, IOptions<VkOptions<KpfuBot>> options, ITimespanRepository<VkUser> users, IUnitOfWorkFactory uowFactory, IVkSenderService vkSenderService) : base("старт")
        {
            _vkApi = vkApi;
            _options = options;
            _users = users;
            _uowFactory = uowFactory;
            _vkSenderService = vkSenderService;
        }

        public override bool CanHandleUpdate(IBot bot, GroupUpdate update)
        {
            if (update.Message != null)
                return update.Message.Text.ToLower().Contains("старт") ||
                       update.Message.Text.ToLower().Contains("начать");
            return false;
        }

        public override async Task<UpdateHandlingResult> HandleCommand(GroupUpdate update)
        {
            using (var uow = _uowFactory.Create())
            {
                try
                {
                    var vkUser = _vkApi.Users.Get(new List<long> {(long) update.Message.FromId}).SingleOrDefault();
                    if (vkUser != null)
                    {
                        var user = new VkUser
                        {
                            ChatState = ChatState.GroupInput,
                            UserId = (long) update.Message.FromId,
                            FirstName = vkUser.FirstName,
                            LastName = vkUser.LastName,
                            ScheduleType = ScheduleType.Text
                        };
                        if (!_users.GetAll().Any(x => x.UserId == update.Message.FromId))
                        {
                            _users.Add(user);
                            uow.Commit();
                        }
                    }
                    
                    var random = new Random();
                    _vkApi.Messages.Send(new MessagesSendParams
                    {
                        UserId = vkUser.Id,
                        Message = $"Привет, {vkUser.FirstName}! Введи номер своей группы в формате **-***",
                        PeerId = _options.Value.GroupId,
                        RandomId = random.Next(int.MaxValue)
                    });
                }
                catch (Exception e)
                {
                    Logger.LogError($"Произошла ошибка в {nameof(HandleCommand)}, {JsonConvert.SerializeObject(e)}");
                    uow.Rollback();
                    await _vkSenderService.SendError((long)update.Message.UserId);
                    return UpdateHandlingResult.Handled;
                }
            }

            return UpdateHandlingResult.Handled;
        }
    }
}
