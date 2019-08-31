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
    public interface ICrudListController<TEntity> : ICrudController<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        ///     Получить список сущности
        /// </summary>
        /// <param name="pageListRequest"> Модель содержащию, мету какую именно информацию нужно вернуть </param>
        /// <returns> Модель сущности </returns>
        [HttpGet]
        Task<PageListResponse<TEntity>> List(
            [FromQuery] PageListRequest pageListRequest);
    }
}