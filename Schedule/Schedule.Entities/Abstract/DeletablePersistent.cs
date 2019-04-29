using System;
using Newtonsoft.Json;
using Storage.Abstractions.Interfaces;

namespace Schedule.Entities.Abstract
{
    public abstract class DeletablePersistent : Persistent, IDeletablePersistent
    {
        /// <summary>
        ///     Дата простановки пометки об удалении. Клиенту не нужно знать об этой информации.
        /// </summary>
        [JsonIgnore] public DateTime? Deleted { get; set; }

        /// <summary>
        ///     Пометка об удалении. Клиенту не нужно знать об этой информации.
        /// </summary>
        [JsonIgnore] public bool IsDeleted { get; set; }
    }
}