using System;

namespace Storage.Abstractions.Interfaces
{
    /// <summary>
    ///     Расширение для <see cref="IPersistent"/>. Используется в случае, когда мы физически не удаляем объект,
    ///     а помечаем об удалении.
    /// </summary>
    public interface IDeletablePersistent : IPersistent
    {
        /// <summary>
        ///     Дата простановки пометки об удалении
        /// </summary>
        DateTime? Deleted { get; set; }

        /// <summary>
        ///     Пометка об удалении.
        /// </summary>
        bool IsDeleted { get; set; }
    }
}