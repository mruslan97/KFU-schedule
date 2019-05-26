using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Schedule.Entities.Kpfu
{
    public class KpfuSubject
    {
        [JsonProperty("subject_name")]
        public string Name { get; set; }

        [JsonProperty("start_day_schedule")]
        public string StartDay { get; set; }

        [JsonProperty("finish_day_schedule")]
        public string EndDay { get; set; }

        [JsonProperty("day_week_schedule")]
        public int DayOfWeek { get; set; }

        [JsonProperty("type_week_schedule")]
        public string WeekType { get; set; }

        [JsonProperty("note_schedule")]
        public string Note { get; set; }

        [JsonProperty("total_time_schedule")]
        public string TotalTime { get; set; }

        [JsonProperty("begin_time_schedule")]
        public string StartTime { get; set; }

        [JsonProperty("end_time_schedule")]
        public string EndTime { get; set; }

        [JsonProperty("teacher_id")]
        public long? TeacherId { get; set; }

        [JsonProperty("teacher_lastname")]
        public string TeacherLastname { get; set; }

        [JsonProperty("teacher_firstname")]
        public string TeacherFirstname { get; set; }

        [JsonProperty("teacher_middlename")]
        public string TeacherMiddlename { get; set; }

        [JsonProperty("num_auditorium_schedule")]
        public string CabinetNumber { get; set; }

        [JsonProperty("building_name")]
        public string BuildingName { get; set; }

        [JsonProperty("building_id")]
        public long BuildingId { get; set; }

        [JsonProperty("subject_kind_name")]
        public string SubjectKindName { get; set; }

        [JsonProperty("subject_id")]
        public string SubjectId { get; set; }

    }
    
    public sealed class KpfuSubjectRoot
    {
        public KpfuSubject[] Subjects { get; set; }
    }
}
