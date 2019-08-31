using System.Linq;
using vm = Schedule.Models;
namespace Schedule.Services
{
    /// <summary> Декоратор запросов для получения сущности </summary>
    public interface IQueryFetchDecorator
    {
        /// <summary> Сдекорировать </summary>
        /// <returns>The decorate.</returns>
        /// <param name="query">Query.</param>
        /// <param name="request">Request.</param>
        /// <typeparam name="TEntity">The 1st type parameter.</typeparam>
        IQueryable<TEntity> Decorate<TEntity>(IQueryable<TEntity> query, vm.PageListRequest request);
    }
}