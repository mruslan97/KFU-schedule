using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Schedule.Entities;
using Schedule.Entities.Kpfu;
using Schedule.Extensions;
using Storage.Abstractions.UnitOfWork;
using vm = Schedule.Models;

namespace Schedule.Services.Impl
{
    public class ScheduleService : IScheduleService
    {
        public IOptions<vm.DomainOptions> Options { get; set; }

        public IMapper Mapper { get; set; }

        public IUnitOfWorkFactory UowFactory { get; set; }

        public ITimespanRepository<Teacher> Teachers { get; set; }

        public ITimespanRepository<Group> Groups { get; set; }

        public ITimespanRepository<Subject> Subjects { get; set; }

        public IHttpClientFactory HttpClientFactory { get; set; }

        public ILogger<ScheduleService> Logger { get; set; }

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
            var groups = await GetGroups();
            Logger.LogInformation($"Загружено {groups.Count} групп");
            var teachers = await GetTeachers();
            Logger.LogInformation($"Загружено {teachers.Count} сотрудников");
            var subjects = await GetSubjects(groups);
            Logger.LogInformation($"Загружено {subjects.Count} уникальных пар");
        }

        private async Task<List<Group>> GetGroups()
        {
            using (var httpClient = HttpClientFactory.CreateClient())
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var encoding = CodePagesEncodingProvider.Instance.GetEncoding(1251);
                var response = await httpClient.GetAsync(
                    $"{Options.Value.KpfuHost}/e-ksu/portal_pg_mobile.get_group_list");
                var json = await response.Content.ReadAsStringAsync();
                var groupsRoot = JsonConvert.DeserializeObject<KpfuGroupRoot>(json);
                var groups = groupsRoot.Groups.Select(group => Mapper.Map<Group>(group)).ToList();
                groups.ForEach(x => UowFactory.Transaction(() => { Groups.Add(x); }));
                
                return Groups.GetAll().ToList();
            }
        }

        private async Task<List<Teacher>> GetTeachers()
        {
            using (var httpClient = HttpClientFactory.CreateClient())
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var encoding = CodePagesEncodingProvider.Instance.GetEncoding(1251);
                var response = await httpClient.GetAsync(
                    $"{Options.Value.KpfuHost}/e-ksu/portal_pg_mobile.get_teacher_list");
                var json = await response.Content.ReadAsStringAsync();
                var teacherRoot = JsonConvert.DeserializeObject<KpfuTeacherRoot>(json);
                var teachers = teacherRoot.Teachers.Select(group => Mapper.Map<Teacher>(group)).ToList();
                teachers.ForEach(x => UowFactory.Transaction(() => { Teachers.Add(x); }));
                
                return teachers;
            }
        }

        private async Task<List<Subject>> GetSubjects(List<Group> groups)
        {
            var allSubjects = new List<Subject>();
            using (var httpClient = HttpClientFactory.CreateClient())
            {
                foreach (var group in groups)
                    try
                    {
                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                        var encoding = CodePagesEncodingProvider.Instance.GetEncoding(1251);
                        var response = await httpClient.GetAsync(
                            $"{Options.Value.KpfuHost}/e-ksu/portal_pg_mobile.get_schedule?p_name_group={group.GroupName}&p_stud_year={Options.Value.Year}&p_stud_semester={Options.Value.Semester}");
                        var json = await response.Content.ReadAsStringAsync();
                        var subjectRoot = JsonConvert.DeserializeObject<KpfuSubjectRoot>(json);
                        var subjects = subjectRoot.Subjects.Select(subject => Mapper.Map<Subject>(subject)).ToList();
                        subjects.ForEach(x =>
                        {
                            x.GroupName = group.GroupName;
                            x.GroupId = group.Id;
                        });
                        allSubjects.AddRange(subjects);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"Ошибка загрузки группы {group.GroupName} {e.Message}");
                    }
                //allSubjects.ForEach(x => UowFactory.Transaction(() => { Subjects.Add(x); }));
                UowFactory.Transaction((() => Subjects.AddRange(allSubjects)));
                
                return allSubjects;
            }
        }
    }
}