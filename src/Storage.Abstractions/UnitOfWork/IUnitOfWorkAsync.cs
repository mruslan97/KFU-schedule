using System;
using System.Threading;
using System.Threading.Tasks;

namespace Storage.Abstractions.UnitOfWork
{
    /// <inheritdoc />
    /// <summary>
    ///     Unit-of-work. Обеспечивает единое изменение всех объектов и откат изменений в случае, если сохранить изменения не удалось
    /// </summary>
    public interface IUnitOfWorkAsync : IDisposable
    {
        /// <summary>
        ///     Сохранить изменения
        /// </summary>
        Task Commit(CancellationToken cancellationToken = default);

        /// <summary>
        ///     Отменить изменения
        /// </summary>
        void Rollback();
    }
}