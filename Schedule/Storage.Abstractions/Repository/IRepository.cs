using System;
using System.Linq;
using JetBrains.Annotations;
using Storage.Abstractions.Interfaces;

namespace Storage.Abstractions.Repository
{
    /// <summary>
    ///     Репозиторий сущности. Оперирует сущностями в базе данных.
    /// </summary>
    /// <typeparam name = "TEntity" > Тип сущности репозитория </typeparam>
    public interface IRepository<TEntity>
        where TEntity : IEntity
    {
        /// <summary>
        ///     Получение конкретной сущности
        /// </summary>
        /// <param name = "id" > Идентификатор сущности </param>
        /// <param name="queryExpression"> Для join объекта </param>
        /// <param name="returnDeleted"> Вернуть удаленный объект (Только для сущностей, который реализуют интерфейс IDeletablePersistent </param>
        /// <param name="returnDeletedChildren"> Не возвращать удаленные сущности, но можно вернуть удаленных детей </param>
        /// <returns> Сущность, соответствующую идентификатору </returns>
        [CanBeNull]
        TEntity Get(long id,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> queryExpression = null,
            bool returnDeleted = false,
            bool returnDeletedChildren = true);

        /// <summary>
        ///     Получение всех сущностей
        /// </summary>
        /// <param name="returnDeleted"> Вернуть удаленный объект (Только для сущностей, который реализуют интерфейс IDeletablePersistent </param>
        /// <param name="returnDeletedChildren"> Не возвращать удаленные сущности, но можно вернуть удаленных детей </param>
        /// <returns> Все сущности </returns>
        IQueryable<TEntity> GetAll(
            bool returnDeleted = false,
            bool returnDeletedChildren = true);

        /// <summary>
        ///     Добавление сущности
        /// </summary>
        /// <param name = "entity" > Добавляемая сущность </param>
        TEntity Add(TEntity entity);

        /// <summary>
        ///     Обновление сущности
        /// </summary>
        /// <param name = "entity" > </param>
        TEntity Update(TEntity entity);

        /// <summary>
        ///     Удаление конкретной сущности
        /// </summary>
        /// <param name = "id" > Идентификатор сущности </param>
        TEntity Delete(long id);

        /// <summary>
        ///     Удаление конкретной сущности
        /// </summary>
        /// <param name = "entity" > Сущность </param>
        TEntity Delete(TEntity entity);
    }
}