using Common;
using Schedule.Entities.Abstract;

namespace Schedule.Entities
{
    public class Teacher : Persistent
    {
        public long KpfuId { get; set; }
        
        [Searchable]
        public string Lastname { get; set; }
       
        public string Firstname { get; set; }
        
        public string Middlename { get; set; }

    }
}