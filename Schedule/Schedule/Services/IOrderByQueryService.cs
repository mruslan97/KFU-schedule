using System.Linq;
using  vm = Schedule.Models;

namespace Schedule.Services
{
    /// <summary> сервис для динамического создания запроса на сортировку данных </summary>
    public interface IOrderByQueryService
    {
        /// <summary> Сдекорировать </summary>
        /// <returns>The decorate.</returns>
        /// <param name="query">Query.</param>
        /// <param name="request">Request.</param>
        /// <typeparam name="TEntity">The 1st type parameter.</typeparam>
        IQueryable<TEntity> Decorate<TEntity>(IQueryable<TEntity> query, vm.PageListRequest request);
    }
}