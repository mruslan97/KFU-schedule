using System;
using Newtonsoft.Json;
using Storage.Abstractions.Interfaces;

namespace Schedule.Entities.Abstract
{
    public class Persistent : Entity, IPersistent
    {
        /// <summary>
        ///     Дата создания. Меняется только при добавлениии. Клиенту не нужно знать об этой информации.
        /// </summary>
        [JsonIgnore] public DateTime? Created { get; set; }

        /// <summary>
        ///     Дата изменения. Меняется только при изменении. Клиенту не нужно знать об этой информации.
        /// </summary>
        [JsonIgnore] public DateTime? Updated { get; set; }
    }
}