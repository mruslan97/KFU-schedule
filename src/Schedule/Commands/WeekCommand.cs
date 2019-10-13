using System;
using System.Linq;
using System.Threading.Tasks;
using FluentDateTime;
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
    public class WeekCommand : CommandBase<DefaultCommandArgs>
    {
        private ITimespanRepository<VkUser> _users;

        private ITimespanRepository<Subject> _subjects;

        /// <summary> паттерн uow </summary>
        private IUnitOfWorkFactory _uowFactory;

        private IVkApi _vkApi;

        private IOptions<VkOptions<KpfuBot>> _options;

        public WeekCommand(ITimespanRepository<Subject> subjects,
            ITimespanRepository<VkUser> users, IUnitOfWorkFactory uowFactory, IVkApi vkApi, IOptions<VkOptions<KpfuBot>> options) : base("На неделю")
        {
            _subjects = subjects;
            _users = users;
            _uowFactory = uowFactory;
            _vkApi = vkApi;
            _options = options;
        }

        public override bool CanHandleUpdate(IBot bot, GroupUpdate update)
        {
            return update.Message != null && update.Message.Text.ToLower().Contains("неделю");
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

            var monday = DateTime.Now.Previous(DayOfWeek.Monday);
            var sunday = DateTime.Now.Previous(DayOfWeek.Sunday);
            var subjects = _subjects.GetAll().Where(x => x.GroupId == user.GroupId 
                                                         && x.StartDay.Value <= monday
                                                         && x.EndDay.Value >= sunday).ToList();
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


            return UpdateHandlingResult.Handled;
        }
    }
}