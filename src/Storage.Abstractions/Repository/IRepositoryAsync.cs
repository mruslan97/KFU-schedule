using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Storage.Abstractions.Interfaces;

namespace Storage.Abstractions.Repository
{
    public interface IRepositoryAsync<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// Получение конкретной сущности
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Сущность, соответствующую идентификатору</returns>
        [ItemCanBeNull]
        Task<TEntity> Get(long id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получение всех сущностей
        /// </summary>
        /// <returns>Все сущности</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// Удаление конкретной сущности
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <param name="token"></param>
        Task Delete(long id, CancellationToken token = default);

        /// <summary>
        /// Удаление конкретной сущности
        /// </summary>
        /// <param name="entity">Сущность</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Добавление сущности
        /// </summary>
        /// <param name="entity">Добавляемая сущность</param>
        /// <param name="token"></param>
        Task Add(TEntity entity, CancellationToken token = default);
    }
}