using Schedule.Entities.Abstract;

namespace Schedule.Entities
{
    public class Group : DeletablePersistent
    {
        public long KpfuId { get; set; }
        public string GroupName { get; set; }
    }
}