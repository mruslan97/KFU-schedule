using System.Collections.Generic;

namespace Schedule.Models
{
    /// <summary> Модель, которая возвращается при пагинации </summary>
    public class PageListResponse<T>
    {
        /// <summary> Список сущностей </summary>
        public IEnumerable<T> Data { get; set; }

        /// <summary> Общее количество </summary>
        public int TotalCount { get; set; }
    }
}