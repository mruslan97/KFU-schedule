using System;
using System.Linq;
using CodeJam.Collections;
using Schedule.Models;

namespace Schedule.Extensions
{
    /// <summary>
    /// Page extensions.
    /// </summary>
    public static class PageExtensions
    {
        /// <summary>
        /// Gets the page model.
        /// </summary>
        /// <returns>The page model.</returns>
        /// <param name="query">Query.</param>
        /// <param name="pageListRequest">Page list request.</param>
        /// <param name="mapper">Mapper.</param>
        /// <typeparam name="TDto">The 1st type parameter.</typeparam>
        /// <typeparam name="TEntity">The 2nd type parameter.</typeparam>
        public static PageListResponse<TDto> GetPageModel<TDto, TEntity>(
            this IQueryable<TEntity> query,
            PageListRequest pageListRequest,
            Func<TEntity, TDto> mapper)
        {
            var count = query.Count();
            var data = query
                .Page(pageListRequest.Page ?? 1, pageListRequest.PageSize ?? 0)
                .ToList()
                .Select(mapper);

            return new PageListResponse<TDto>
            {
                Data = data,
                TotalCount = count
            };
        }

        /// <summary>
        /// Gets the page model.
        /// </summary>
        /// <returns>The page model.</returns>
        /// <param name="query">Query.</param>
        /// <param name="pageListRequest">Page list request.</param>
        /// <typeparam name="TEntity">The 1st type parameter.</typeparam>
        public static PageListResponse<TEntity> GetPageModel<TEntity>(
            this IQueryable<TEntity> query,
            PageListRequest pageListRequest)
        {
            var count = query.Count();
            var data = query
                .Page(pageListRequest.Page ?? 1, pageListRequest.PageSize ?? 0)
                .ToList();

            return new PageListResponse<TEntity>
            {
                Data = data,
                TotalCount = count
            };
        }

        /// <summary> Преобразовать модель ответа на другой </summary>
        /// <returns>The data.</returns>
        /// <param name="data">Data.</param>
        /// <param name="convert">Convert.</param>
        /// <typeparam name="TResult">The 1st type parameter.</typeparam>
        /// <typeparam name="TEntity">The 2nd type parameter.</typeparam>
        public static PageListResponse<TResult> ConvertData<TResult, TEntity>(
           this PageListResponse<TEntity> data,
           Func<TEntity, TResult> convert)
        {
            return new PageListResponse<TResult>
            {
                Data = data.Data.Select(convert),
                TotalCount = data.TotalCount
            };
        }
    }
}