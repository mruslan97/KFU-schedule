using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storage.Abstractions.Interfaces;

namespace Schedule.Controllers.Base
{
    /// <summary>
    ///     Контроллер который выполняет CRUD операции над моделями.
    /// </summary>
    /// <typeparam name="TEntity"> Сущность для которой создаются CRUD операции </typeparam>
    [Route("api/[controller]"), Produces("application/json"), AllowAnonymous]
    public abstract class BaseCrudController<TEntity> : BaseCrudController<TEntity, TEntity>
        where TEntity : class, IEntity
    {
        /// <summary> Маппинг из entity в dto. Т.к. тип entity и dto совпадает нет смысла их маппить c помощью маппера </summary>
        /// <returns>The entity.</returns>
        /// <param name="entity">Entity.</param>
        protected override TEntity ToEntity(TEntity entity)
        {
            return entity;
        }

        /// <summary> Маппинг из dto в entity. Т.к. тип entity и dto совпадает нет смысла их маппить c помощью маппера </summary>
        /// <returns>The entity.</returns>
        /// <param name="dto">Dto.</param>
        protected override TEntity ToDto(TEntity dto)
        {
            return dto;
        }
    }
}