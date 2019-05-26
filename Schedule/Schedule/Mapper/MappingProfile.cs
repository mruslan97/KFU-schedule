using System;
using System.Globalization;
using AutoMapper;
using Schedule.Entities;
using Schedule.Entities.Kpfu;
using vm = Schedule.Models;

namespace Schedule.Mapper
{
    /// <summary>
    ///     Маппинг моделей 
    /// </summary>
    public class MappingProfile : Profile
    {
        private string[] _timeFormats => new[] { "HH:mm", "HH.mm" };
        private readonly string _hostName;
        public MappingProfile(vm.DomainOptions options)
        {
            // для универсальности hostName должен всегда заканчиваться с '/'
            _hostName = options?.HostName.EndsWith('/') ?? false ? options.HostName : $"{options?.HostName}/";

            CreateMap<string, TimeSpan?>().ConvertUsing<DateTimeTypeConverter>();
            CreateMap<KpfuSubject, Subject>()
                .ForMember(x => x.KpfuId, x => x.MapFrom(s => s.SubjectId));
            CreateMap<KpfuGroup, Group>()
                .ForMember(x => x.KpfuId, x => x.MapFrom(s => s.GroupId));
            CreateMap<KpfuTeacher, Teacher>()
                .ForMember(x => x.KpfuId, x => x.MapFrom(s => s.TeacherId));

        }
    }
}