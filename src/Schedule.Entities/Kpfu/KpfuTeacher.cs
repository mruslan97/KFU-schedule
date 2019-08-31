using Newtonsoft.Json;

namespace Schedule.Entities.Kpfu
{
    public class KpfuTeacher
    {
        [JsonProperty("id")]
        public long TeacherId { get; set; }
        
        [JsonProperty("lastname")]
        public string Lastname { get; set; }
        
        [JsonProperty("firstname")]
        public string Firstname { get; set; }
        
        [JsonProperty("middlename")]
        public string Middlename { get; set; }
    }
    
    public sealed class KpfuTeacherRoot
    {
        [JsonProperty("teachers")]
        public KpfuTeacher[] Teachers { get; set; }
    }
}