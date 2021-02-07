using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CodeJam.Collections;
using CodeJam.Strings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog.Fluent;
using Schedule.Entities;
using Schedule.Entities.Kpfu;
using Schedule.Extensions;
using Storage.Abstractions.UnitOfWork;
using vm = Schedule.Models;

namespace Schedule.Services.Impl
{
    /// <inheritdoc />
    public class ScheduleService : IScheduleService
    {
        #region di properties

        public IOptions<vm.DomainOptions> Options { get; set; }

        public IMapper Mapper { get; set; }

        public IUnitOfWorkFactory UowFactory { get; set; }

        public ITimespanRepository<Teacher> Teachers { get; set; }

        public ITimespanRepository<Group> Groups { get; set; }

        public ITimespanRepository<Subject> Subjects { get; set; }

        public IHttpClientFactory HttpClientFactory { get; set; }

        public ILogger<ScheduleService> Logger { get; set; }
        
        public IParserService ParserService { get; set; }

        #endregion

        /// <inheritdoc />
        public async Task InitializeLocalStorage()
        {
            try
            {
                Logger.LogInformation($"Выполняется очистка базы данных");
                using (var uow = UowFactory.Create())
                {
                    Subjects.DeleteRange(Subjects.GetAll());
                    Teachers.DeleteRange(Teachers.GetAll());
                    Groups.DeleteRange(Groups.GetAll());

                    uow.Commit();
                }
            }
            catch (Exception e)
            {
                Logger.LogError(JsonConvert.SerializeObject(e));
            }
            
            Logger.LogInformation($"Очистка базы данных завершена");
            var groups = await SaveGroups();
            Logger.LogInformation($"Загружено {groups.Count} групп");
            var teachers = await SaveTeachers();
            Logger.LogInformation($"Загружено {teachers.Count} сотрудников");
            var subjects = await SaveSubjects(groups);
            Logger.LogInformation($"Загружено {subjects.Count} уникальных пар");
        }

        /// <inheritdoc />
        public async Task UpdateSubjects()
        {
            var newGroups = await ParserService.GetGroups();

            var groups = Groups.GetAll().ToList();
            var uniqueNewGroups = newGroups.ExceptBy(groups, x => x.GroupName).ToList();

            if (uniqueNewGroups.Count > 0)
            {
                UowFactory.Transaction(() => Groups.AddRange(uniqueNewGroups));
                Logger.LogInformation($"Сохранено {uniqueNewGroups.Count} новых групп");
            }

            using (var httpClient = HttpClientFactory.CreateClient())
            {
                foreach (var group in groups)
                {
                    Logger.LogInformation($"Выполняется обновление для группы {group.GroupName}");
                    try
                    {
                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                        // Конвертируем в unicode для совместимости с русскими символами
                        var unicodeGroupName = string.Concat(group.GroupName.Select(c => $@"\u{(int)c:x4}"));
                        var response = await httpClient.GetAsync(
                            $"{Options.Value.KpfuHost}/e-ksu/portal_pg_mobile.get_schedule?p_name_group={unicodeGroupName}&p_stud_year={Options.Value.Year}&p_stud_semester={Options.Value.Semester}");
                        var json = await response.Content.ReadAsStringAsync();
                        var subjectRoot = JsonConvert.DeserializeObject<KpfuSubjectRoot>(json);
                        var subjects = subjectRoot.Subjects.Select(subject => Mapper.Map<Subject>(subject)).ToList();
                        subjects.ForEach(x =>
                        {
                            x.GroupName = group.GroupName;
                            x.GroupId = group.Id;
                        });
                        using (var uow = UowFactory.Create())
                        {
                            Subjects.DeleteRange(Subjects.GetAll().Where(x => x.GroupId == group.Id));
                            Subjects.AddRange(subjects);
                            uow.Commit();
                        }
                    }
                    catch (JsonReaderException e)
                    {
                        Logger.LogError($"Невалидный json, группа {group.GroupName} {e.Message}");
                        Logger.LogError(JsonConvert.SerializeObject(e));
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"Ошибка обновления группы {group.GroupName}");
                        Logger.LogError(JsonConvert.SerializeObject(e));
                    }
                }
            }
        }

        /// <inheritdoc />
        public async Task UpdateLocalDb()
        {
            await MergeGroups();
            await MergeTeachers();
            await MergeSubjects();
        }
        
        #region Get and parse schedule data

        private async Task<List<Group>> SaveGroups()
        {
            var groups = await ParserService.GetGroups();
            groups.ForEach(x => UowFactory.Transaction(() => { Groups.Add(x); }));
                
            return Groups.GetAll().ToList();
        }

        private async Task<List<Teacher>> SaveTeachers()
        {
            var teachers = await ParserService.GetTeachers();
            teachers.ForEach(x => UowFactory.Transaction(() => { Teachers.Add(x); }));
                
            return teachers;
        }

        private async Task<List<Subject>> SaveSubjects(List<Group> groups)
        {
            var subjects = await ParserService.GetSubjects(groups);
            subjects.ForEach(x => UowFactory.Transaction(() => { Subjects.Add(x); }));

            return subjects;
        }

        #endregion

        #region Merge data from site and db

        private async Task MergeGroups()
        {
            var groupsFromSite = await ParserService.GetGroups();
            var localGroups = Groups.GetAll();

            var onlyNewGroups = groupsFromSite.ExceptBy(localGroups, x => x.GroupName).ToList();
            try
            {
                Logger.LogInformation($"Выполняется мерж групп, количество новых: {onlyNewGroups.Count} ");
                UowFactory.Transaction(() => { Groups.AddRange(onlyNewGroups); });
            }
            catch (Exception e)
            {
                Logger.LogError("Ошибка мержа групп");
                Logger.LogError(JsonConvert.SerializeObject(e));
            }
        }

        private async Task MergeTeachers()
        {
            var teachersFromSite = await ParserService.GetTeachers();
            var localTeachers = Teachers.GetAll();

            var onlyNewTeachers = teachersFromSite.ExceptBy(localTeachers, x => x.KpfuId).ToList();
            try
            {
                Logger.LogInformation($"Выполняется мерж преподавателей, количество новых: {onlyNewTeachers.Count()}");
                UowFactory.Transaction(() => { Teachers.AddRange(onlyNewTeachers); });
            }
            catch (Exception e)
            {
                Logger.LogError("Ошибка мержа преподавателей");
                Logger.LogError(JsonConvert.SerializeObject(e));
            }
        }

        private async Task MergeSubjects()
        {
            // Удаляем существующие предметы, т.к. они не имеют внешних зависимостей. 
            UowFactory.Transaction((() =>
            {
                var subjectsId = Subjects.GetAll().Select(x => x.Id);
                subjectsId.ForEachAsync(x => Subjects.Delete(x));
            }));
            var groups = Groups.GetAll();
            
            var subjects = await ParserService.GetSubjects(groups);
            try
            {
                Logger.LogInformation($"Выполняется загрузка новых предметов, количество новых: {subjects.Count()}");
                UowFactory.Transaction(() => { Subjects.AddRange(subjects); });
            }
            catch (Exception e)
            {
                Logger.LogError("Ошибка загрузки новых предметов");
                Logger.LogError(JsonConvert.SerializeObject(e));
            }
            
        }

        #endregion
        
    }
}