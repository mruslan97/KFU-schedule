using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Schedule.Models;
using Storage.Abstractions.Interfaces;
using Schedule.Extensions;
using ExpressionExtensions = Schedule.Extensions.ExpressionExtensions;

namespace Schedule.Services.Impl.QueryDecorators
{
    /// <inheritdoc />
    public class OrderByQueryDecorator : IQueryFetchDecorator
    {
        /// <summary> логгер </summary>
        public ILogger<OrderByQueryDecorator> Logger { get; set; }

        /// <inheritdoc />
        public IQueryable<TEntity> Decorate<TEntity>(IQueryable<TEntity> query, PageListRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.OrderBy))
            {
                var (expr, isValid) = Made<TEntity>(request.OrderBy);

                if (isValid)
                {
                    return request.ByDescending
                        ? query.OrderByDescending(expr)
                        : query.OrderBy(expr);
                }

                Logger.LogError($"Не валидное выражение для сортировки: {request.OrderBy}");
            }

            return typeof(IPersistent).IsAssignableFrom(typeof(TEntity))
                ? query.OrderByDescending(x => ((IPersistent)x).Created)
                : query;
        }

        /// <summary> Создать выражение для сортировки </summary>
        /// <returns>The made.</returns>
        /// <param name="property">Property.</param>
        /// <typeparam name="TEntity">The 1st type parameter.</typeparam>
        public (Expression<Func<TEntity, object>> expr, bool isValid) Made<TEntity>(string property)
        {
            var parameter = Expression.Parameter(typeof(TEntity));
            var (propertyType, propertyExpression, isValid) =
                ExpressionExtensions.BuildPath(property, parameter, typeof(TEntity));

            if (!isValid)
            {
                return (null, false);
            }

            var expressionProperty = Expression.Convert(propertyExpression, typeof(object));

            return (Expression.Lambda<Func<TEntity, object>>(expressionProperty, parameter), true);
        }
    }
}