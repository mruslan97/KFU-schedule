using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using CodeJam.Collections;
using CodeJam.Strings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Schedule.Entities;
using Schedule.Entities.Enums;
using Schedule.Extensions;
using Schedule.Models;
using Schedule.Services;
using Storage.Abstractions.UnitOfWork;
using Vk.Bot.Framework;
using Vk.Bot.Framework.Abstractions;
using VkNet.Abstractions;
using VkNet.Model.GroupUpdate;
using VkNet.Model.RequestParams;

namespace Schedule.Commands
{
    public class TeacherSearchCommand : CommandBase<DefaultCommandArgs>
    {
        private ITimespanRepository<VkUser> _users;

        private ITimespanRepository<Subject> _subjects;

        private ITimespanRepository<Teacher> _teachers;

        /// <summary> паттерн uow </summary>
        private IUnitOfWorkFactory _uowFactory;

        private IVkApi _vkApi;

        private IOptions<VkOptions<KpfuBot>> _options;


        public TeacherSearchCommand(ITimespanRepository<Subject> subjects,
            ITimespanRepository<VkUser> users, IUnitOfWorkFactory uowFactory, IVkApi vkApi, IOptions<VkOptions<KpfuBot>> options, ITimespanRepository<Teacher> teachers) : base("поиск")
        {
            _subjects = subjects;
            _users = users;
            _uowFactory = uowFactory;
            _vkApi = vkApi;
            _options = options;
            _teachers = teachers;
        }

        public override bool CanHandleUpdate(IBot bot, GroupUpdate update)
        {
            var user = _users.GetAll().FirstOrDefault(x => x.UserId == update.Message.FromId);
            
            return update.Message != null && update.Message.Text.ToLower().Contains("поиск преподавателя") 
                   || user != null && user.ChatState == ChatState.TeacherSearch;
        }

        public override async Task<UpdateHandlingResult> HandleCommand(GroupUpdate update)
        {
            using (var uow = _uowFactory.Create())
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

                if (!update.Message.Payload.IsNullOrWhiteSpace() && update.Message.Payload.Contains("teacherId"))
                {
                    var teacherId = JToken.Parse(update.Message.Payload)["teacherId"].Value<long>();
                    var subjects = _subjects.GetAll()
                        .Where(x => x.TeacherId == teacherId).ToList();
                    var daysOfWeek = subjects.OrderBy(x => x.DayOfWeek).Select(x => x.DayOfWeek).Distinct();
                    foreach (var day in daysOfWeek)
                    {
                        _vkApi.Messages.Send(new MessagesSendParams
                        {
                            UserId = user.UserId,
                            Message = subjects.Where(x => x.DayOfWeek == day).DistinctBy(x => x.TotalTime).ToMessage((DayOfWeek)day, true),
                            PeerId = _options.Value.GroupId,
                            RandomId = random.Next(int.MaxValue),
                            Keyboard = MessageDecorator.ReturnMenu()
                        });
                    }

                    return UpdateHandlingResult.Handled;
                }

                if (user.ChatState != ChatState.TeacherSearch)
                {

                    _vkApi.Messages.Send(new MessagesSendParams
                    {
                        UserId = update.Message.FromId,
                        Message = $"Введи фамилию преподавателя",
                        PeerId = _options.Value.GroupId,
                        RandomId = random.Next(int.MaxValue)
                    });
                    user.ChatState = ChatState.TeacherSearch;
                    _users.Update(user);
                    uow.Commit();

                    return UpdateHandlingResult.Handled;
                }

                var teachers = _teachers.GetAll(new PageListRequest
                {
                    Search = update.Message.Text
                }).ToList();

                if (teachers.Count > 0)
                {
                    _vkApi.Messages.Send(new MessagesSendParams
                    {
                        UserId = user.UserId,
                        Message = $"Найдено {teachers.Count} совпадений. Выбери преподавателя из списка",
                        PeerId = _options.Value.GroupId,
                        RandomId = random.Next(int.MaxValue),
                        Keyboard = teachers.BuildKeyboard()
                    });
                }
                else
                {
                    _vkApi.Messages.Send(new MessagesSendParams
                    {
                        UserId = user.UserId,
                        Message = "Не удалось найти преподавателя",
                        PeerId = _options.Value.GroupId,
                        RandomId = random.Next(int.MaxValue),
                        Keyboard = MessageDecorator.ReturnMenu()
                    });
                }
            }

            return UpdateHandlingResult.Handled;
        }
    }
}