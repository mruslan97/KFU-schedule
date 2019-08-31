﻿using System;
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
    public class UpdateService : IUpdateService
    {
        public IOptions<vm.DomainOptions> Options { get; set; }

        public IMapper Mapper { get; set; }

        public IUnitOfWorkFactory UowFactory { get; set; }

        public ITimespanRepository<Group> Groups { get; set; }

        public ITimespanRepository<Subject> Subjects { get; set; }
        public IHttpClientFactory HttpClientFactory { get; set; }

        public ILogger<ScheduleService> Logger { get; set; }
        
        public async Task UpdateLocaleStorage()
        {
            var groups = Groups.GetAll().ToList();
            using (var httpClient = HttpClientFactory.CreateClient())
            {
                foreach (var group in groups)
                {
                    Logger.LogInformation($"Выполняется обновление для группы {group.GroupName}");
                    try
                    {
                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
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
                        using (var uow = UowFactory.Create())
                        {
                            Subjects.DeleteRange(Subjects.GetAll().Where(x => x.GroupId == group.Id));
                            Subjects.AddRange(subjects);
                            uow.Commit();
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"Ошибка обновления группы {group.GroupName}");
                        Logger.LogError(JsonConvert.SerializeObject(e));
                    }
                }
            }
        }
    }
}