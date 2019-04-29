using System;

namespace Storage.Abstractions.Interfaces
{
    /// <summary>
    ///     Все сущности в БД имеют дату создания и дату изменения.
    ///     DateCreate меняется только при добавлениии.
    ///     DateChange меняется только при изменении.
    /// 
    ///     При наследовании использовать тип {Persistent} 
    ///  
    ///     Для типов, которые не удаляются, а помечаются удаленным необъодимо использовать
    ///     <see cref="IDeletablePersistent"/>
    /// </summary>
    public interface IPersistent : IEntity
    {
        /// <summary>
        ///     Дата создания. Меняется только при добавлениии.
        /// </summary>
        DateTime? Created { get; set; }

        /// <summary>
        ///     Дата изменения. Меняется только при изменении.
        /// </summary>
        DateTime? Updated { get; set; }
    }
}