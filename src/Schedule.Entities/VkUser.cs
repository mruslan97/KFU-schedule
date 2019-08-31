using Schedule.Entities.Abstract;
using Schedule.Entities.Enums;

namespace Schedule.Entities
{
    /// <summary> Пользователь vk.com </summary>
    public class VkUser : DeletablePersistent
    {
        /// <summary> id вконтакте </summary>
        public long UserId { get; set; }

        /// <summary> Имя </summary>
        public string FirstName { get; set; }

        /// <summary> Фамилия </summary>
        public string LastName { get; set; }

        /// <summary> Группа </summary>
        public Group Group { get; set; }

        /// <summary> Группа </summary>
        public long? GroupId { get; set; }

        /// <summary> Состояние чата </summary>
        public ChatState ChatState { get; set; }

        /// <summary> Формат расписания </summary>
        public ScheduleType ScheduleType { get; set; }
    }
}