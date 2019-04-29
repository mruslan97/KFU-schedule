using System;
using System.Linq;
using JetBrains.Annotations;
using Schedule.Models;
using Storage.Abstractions.Interfaces;
using Storage.Abstractions.Repository;

namespace Schedule.Services
{
    /// <summary>
    ///     Репозиторий обертка. Нужно чтобы при выполнении сохранения делать фиксации времени изменения.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности</typeparam>
    public interface ITimespanRepository<TEntity> : IRepository<TEntity>
        where TEntity : IEntity
    {
        /// <summary>
        ///     Получение конкретной сущности
        /// </summary>
        /// <param name="request"> Модель содержайщий идентификатор и признак удаления </param>
        /// <param name="joins"> Для join объекта </param>
        /// <returns> Сущность, соответствующую идентификатору </returns>
        [CanBeNull]
        TEntity Get(
            ItemRequest request,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> joins = null);

        /// <summary>
        ///     Получение конкретной сущности
        /// </summary>
        /// <param name="pageListRequest"> Модель содержащию, мету какую именно информацию нужно вернуть </param>
        /// <returns> Сущность, соответствующую идентификатору </returns>
        IQueryable<TEntity> GetAll(PageListRequest pageListRequest);
    }
}