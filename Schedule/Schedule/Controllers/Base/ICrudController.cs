using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Schedule.Models;
using Storage.Abstractions.Interfaces;

namespace Schedule.Controllers.Base
{
    /// <summary>
    ///     Контроллер который выполняет CRUD операции над моделями.
    /// </summary>
    /// <typeparam name="TEntity">Сущность для которой создаются CRUD операции</typeparam>
    public interface ICrudController<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        ///     Получить сущность по идентификатору.
        /// </summary>
        /// <param name="request"> Модель содержайщий идентификатор и признак удаления </param>
        /// <returns> Модель сущности </returns>
        [HttpGet("{id}")] Task<TEntity> Get(ItemRequest request);

        /// <summary>
        ///     Добавление сущности
        /// </summary>
        /// <param name="entity"> Сущность, которую нужно добавить</param>
        /// <returns> Модель добавленной сущности </returns>
        [HttpPost] Task<TEntity> Add([FromBody] TEntity entity);

        /// <summary>
        ///     Обновление сущности.
        /// </summary>
        /// <param name="entity"> Сущность, которую нужно добавить</param>
        /// <returns> Модель обновленной сущности </returns>
        [HttpPut] Task<TEntity> Update([FromBody] TEntity entity);

        /// <summary>
        ///     Удаление сущности.
        /// </summary>
        /// <param name="id"> Идентификатор сущности </param>
        /// <returns> Модель удаленной сущности </returns>
        [HttpDelete("{id}")] Task<TEntity> Delete([FromRoute] long id);
    }
}