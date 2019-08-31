using System.Collections.Generic;
using Newtonsoft.Json;

namespace Schedule.Models
{
    /// <summary> Модель, содержащая информация для возврата страниц </summary>
    public class PageListRequest
    {
        /// <summary> Берется из query params. Сравнение ключей
        /// делается без учета регистра. </summary>
        [JsonIgnore] public Dictionary<string, QueryParameter> Where { get; set; } = new Dictionary<string, QueryParameter>();

        /// <summary> Строка поиска </summary>
        public string Search { get; set; }

        /// <summary> Страница, которую нужно вернуть </summary>
        public int? Page { get; set; }

        /// <summary> Размер страницы </summary>
        public int? PageSize { get; set; }

        /// <summary> Вернуть удаленные объекты </summary>
        [JsonIgnore] public bool ReturnDeleted { get; set; }

        /// <summary> Вернуть удаленный объект, но все связанные удаленные вернуть </summary>
        [JsonIgnore] public bool ReturnDeletedChildren { get; set; } = true;

        /// <summary> По какому полю нужно сортировать </summary>
        public string OrderBy { get; set; }

        /// <summary> Отсортировать в порядке убывания </summary>
        public bool ByDescending { get; set; }
    }
}