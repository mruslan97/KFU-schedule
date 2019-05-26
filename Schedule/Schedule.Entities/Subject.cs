using System;
using Common;
using Schedule.Entities.Abstract;
using Schedule.Entities.Enums;

namespace Schedule.Entities
{
    public class Subject : DeletablePersistent
    {
        public string Name { get; set; }

        
        public DateTime? StartDay { get; set; }

        
        public DateTime? EndDay { get; set; }

       
        public int DayOfWeek { get; set; }

        
        public WeekType WeekType { get; set; }

        
        public string Note { get; set; }

        
        public string TotalTime { get; set; }

        
        public TimeSpan? StartTime { get; set; }

        
        public TimeSpan? EndTime { get; set; }

        
        public long? TeacherId { get; set; }

        [Searchable]
        public string TeacherLastname { get; set; }

        [Searchable]
        public string TeacherFirstname { get; set; }

        [Searchable]
        public string TeacherMiddlename { get; set; }

        
        public string CabinetNumber { get; set; }

       
        public string BuildingName { get; set; }

        
        public long BuildingId { get; set; }

        
        public string SubjectKindName { get; set; }

        [Searchable]
        public string GroupName { get; set; }

        public Group Group { get; set; }

        public long GroupId { get; set; }

        public long KpfuId { get; set; }
    }
}
