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
using Storage.Abstractions.UnitOfWork;
using vm = Schedule.Models;

namespace Schedule.Services.Impl
{
    /// <inheritdoc />
    public class ParserService : IParserService
    {
        public IOptions<vm.DomainOptions> Options { get; set; }

        public IMapper Mapper { get; set; }

        public IHttpClientFactory HttpClientFactory { get; set; }

        public ILogger<ScheduleService> Logger { get; set; }
        
        /// <inheritdoc />
        public async Task<List<Group>> GetGroups()
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

                return groups;
            }
        }

        /// <inheritdoc />
        public async Task<List<Teacher>> GetTeachers()
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

                return teachers;
            }
        }

        /// <inheritdoc />
        public async Task<List<Subject>> GetSubjects(IEnumerable<Group> groups)
        {
            var allSubjects = new List<Subject>();
            using (var httpClient = HttpClientFactory.CreateClient())
            {
                foreach (var group in groups)
                    try
                    {
                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                        // Конвертируем в unicode для совместимости с русскими символами
                        var unicodeGroupName = string.Concat(group.GroupName.Select(c => $@"\u{(int) c:x4}"));
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
                        allSubjects.AddRange(subjects);
                    }
                    catch (JsonReaderException e)
                    {
                        Logger.LogError($"Невалидный json, группа {group.GroupName} {e.Message}");
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"Ошибка загрузки группы {group.GroupName} {e.Message}");
                    }

                return allSubjects;
            }
        }
    }
}