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
    public class SetupGroupCommand : CommandBase<DefaultCommandArgs>
    {
        private ITimespanRepository<VkUser> _users;

        private ITimespanRepository<Group> _groups;

        /// <summary> паттерн uow </summary>
        private IUnitOfWorkFactory _uowFactory;

        private IVkApi _vkApi;

        private IOptions<VkOptions<KpfuBot>> _options;

        private IVkSenderService _vkSenderService;

        private ILogger<SetupGroupCommand> _logger;

        public SetupGroupCommand(IVkApi vkApi, IOptions<VkOptions<KpfuBot>> options, 
            ITimespanRepository<VkUser> users, IUnitOfWorkFactory uowFactory, 
            ITimespanRepository<Group> groups, IVkSenderService vkSenderService, ILogger<SetupGroupCommand> logger) : base("старт")
        {
            _vkApi = vkApi;
            _options = options;
            _users = users;
            _uowFactory = uowFactory;
            _groups = groups;
            _vkSenderService = vkSenderService;
            _logger = logger;
        }

        public override bool CanHandleUpdate(IBot bot, GroupUpdate update)
        {
            return _groups.GetAll().Any(x => update.Message.Text.Contains(x.GroupName));
        }

        public override async Task<UpdateHandlingResult> HandleCommand(GroupUpdate update)
        {
            using (var uow = _uowFactory.Create())
            {
                try
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
                        return UpdateHandlingResult.Handled;
                    }

                    user.Group = _groups.GetAll().FirstOrDefault(x => update.Message.Text.Contains(x.GroupName));
                    user.ChatState = ChatState.MainMenu;
                    _users.Update(user);
                    uow.Commit();
                    _vkApi.Messages.Send(new MessagesSendParams
                    {
                        UserId = user.UserId,
                        Message = $"Группа успешно сохранена. В настройках можно выбрать формат расписания, текст или картинка.",
                        PeerId = _options.Value.GroupId,
                        RandomId = random.Next(int.MaxValue),
                        Keyboard = MessageDecorator.BuildMainMenu()
                    });
                }
                catch (Exception e)
                {
                    _logger.LogError($"Произошла ошибка в {nameof(HandleCommand)}, {JsonConvert.SerializeObject(e)}");
                    uow.Rollback();
                    await _vkSenderService.SendError((long)update.Message.UserId);
                    return UpdateHandlingResult.Handled;
                }
            }

            return UpdateHandlingResult.Handled;
        }
    }
}
