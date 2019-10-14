using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Schedule.Entities;
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
    public class SettingsCommand : CommandBase<DefaultCommandArgs>
    {
        private ITimespanRepository<VkUser> _users;

        private ITimespanRepository<Subject> _subjects;

        /// <summary> паттерн uow </summary>
        private IUnitOfWorkFactory _uowFactory;

        private IVkApi _vkApi;

        private IOptions<VkOptions<KpfuBot>> _options;

        private IVkSenderService _vkSenderService;

        private ILogger<MainMenuCommand> _logger;

        public SettingsCommand(ITimespanRepository<Subject> subjects,
            ITimespanRepository<VkUser> users, IUnitOfWorkFactory uowFactory, 
            IVkApi vkApi, IOptions<VkOptions<KpfuBot>> options, 
            IVkSenderService vkSenderService, 
            ILogger<MainMenuCommand> logger) : base("Назад")
        {
            _subjects = subjects;
            _users = users;
            _uowFactory = uowFactory;
            _vkApi = vkApi;
            _options = options;
            _vkSenderService = vkSenderService;
            _logger = logger;
        }
        
        public override bool CanHandleUpdate(IBot bot, GroupUpdate update)
        {
            return update.Message != null && update.Message.Text.ToLower().Contains("настройки");
        }

        public override Task<UpdateHandlingResult> HandleCommand(GroupUpdate update)
        {
            var random = new Random();
            _vkApi.Messages.Send(new MessagesSendParams
            {
                UserId = update.Message.FromId,
                Message = "Здесь можно выбрать формат расписания. Текст/картинка",
                PeerId = _options.Value.GroupId,
                RandomId = random.Next(int.MaxValue),
                Keyboard = MessageDecorator.BuildSettingsMenu()
            });
            
            return Task.FromResult(UpdateHandlingResult.Handled);
        }
    }
}