using System;

namespace Storage.Abstractions.UnitOfWork
{
    /// <summary>
    ///     Unit-of-work. Обеспечивает единое изменение всех объектов и откат изменений в случае, если сохранить изменения не
    ///     удалось
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        ///     Сохранить изменения
        /// </summary>
        void Commit();

        /// <summary> Сохраняет, но не закрывает транзакуию. </summary>
        void Save();

        /// <summary>
        ///     Отменить изменения
        /// </summary>
        void Rollback();
    }
}