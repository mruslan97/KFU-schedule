using System;
using System.Linq;
using System.Threading.Tasks;
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
using VkNet.Model.GroupUpdate;
using VkNet.Model.RequestParams;

namespace Schedule.Commands
{
    public class ChangeScheduleFormatCommand : CommandBase<DefaultCommandArgs>
    {
        private ITimespanRepository<VkUser> _users;

        /// <summary> паттерн uow </summary>
        private IUnitOfWorkFactory _uowFactory;

        private IVkApi _vkApi;

        private IOptions<VkOptions<KpfuBot>> _options;

        private IVkSenderService _vkSenderService;

        private ILogger<MainMenuCommand> _logger;

        public ChangeScheduleFormatCommand(
            ITimespanRepository<VkUser> users, IUnitOfWorkFactory uowFactory, 
            IVkApi vkApi, IOptions<VkOptions<KpfuBot>> options, 
            IVkSenderService vkSenderService, 
            ILogger<MainMenuCommand> logger) : base("картинка")
        {
            _users = users;
            _uowFactory = uowFactory;
            _vkApi = vkApi;
            _options = options;
            _vkSenderService = vkSenderService;
            _logger = logger;
        }
        
        public override bool CanHandleUpdate(IBot bot, GroupUpdate update)
        {
            return update.Message != null && update.Message.Payload.Contains("set_button", StringComparison.InvariantCultureIgnoreCase);
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

                    user.ScheduleType = update.Message.Text.Contains("картинка", StringComparison.InvariantCultureIgnoreCase)
                        ? ScheduleType.Image
                        : ScheduleType.Text;
                    _users.Update(user);
                    uow.Commit();
                    _vkApi.Messages.Send(new MessagesSendParams
                    {
                        UserId = user.UserId,
                        Message = $"Новый формат {update.Message.Text} успешно сохранен.",
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