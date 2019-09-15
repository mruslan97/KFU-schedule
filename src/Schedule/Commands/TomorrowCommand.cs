using System;
using System.Linq;
using System.Threading.Tasks;
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
    public class TomorrowCommand : CommandBase<DefaultCommandArgs>
    {
        private ITimespanRepository<VkUser> _users;

        private ITimespanRepository<Subject> _subjects;

        /// <summary> паттерн uow </summary>
        private IUnitOfWorkFactory _uowFactory;

        private IVkApi _vkApi;

        private IOptions<VkOptions<KpfuBot>> _options;

        public TomorrowCommand(ITimespanRepository<Subject> subjects,
            ITimespanRepository<VkUser> users, IUnitOfWorkFactory uowFactory, IVkApi vkApi, IOptions<VkOptions<KpfuBot>> options) : base("На завтра")
        {
            _subjects = subjects;
            _users = users;
            _uowFactory = uowFactory;
            _vkApi = vkApi;
            _options = options;
        }

        public override bool CanHandleUpdate(IBot bot, GroupUpdate update)
        {
            return update.Message != null && update.Message.Text.ToLower().Contains("завтра");
        }

        public override async Task<UpdateHandlingResult> HandleCommand(GroupUpdate update)
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

            var dayOfWeek = DateTime.Now.DayOfWeek == (DayOfWeek) 6 ? 1 : (int) DateTime.Now.DayOfWeek + 1;
            var subjects = _subjects.GetAll().Where(x => x.GroupId == user.GroupId && x.DayOfWeek == dayOfWeek)
                .ToList();

            _vkApi.Messages.Send(new MessagesSendParams
            {
                UserId = user.UserId,
                Message = subjects.ToMessage((DayOfWeek) dayOfWeek),
                PeerId = _options.Value.GroupId,
                RandomId = random.Next(int.MaxValue),
            });


            return UpdateHandlingResult.Handled;
        }
    }
}