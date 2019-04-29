using Newtonsoft.Json;

namespace Schedule.Entities.Kpfu
{
    public class KpfuGroup
    {
        [JsonProperty("group_id")]
        public long GroupId { get; set; }
        
        [JsonProperty("group_name")]
        public string GroupName { get; set; }
    }

    public sealed class KpfuGroupRoot
    {
        [JsonProperty("group_list")]
        public KpfuGroup[] Groups { get; set; }
    }
}