using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Common;
using Microsoft.Extensions.Logging;
using Schedule.Models;

namespace Schedule.Services.Impl.QueryDecorators
{
    /// <inheritdoc />
    public class SearchQueryDecorator : IQueryFetchDecorator
    {
        /// <summary> логгер </summary>
        public ILogger<SearchQueryDecorator> Logger { get; set; }

        /// <inheritdoc />
        public IQueryable<TEntity> Decorate<TEntity>(IQueryable<TEntity> query, PageListRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchWhere = Made<TEntity>(request.Search);

                if (searchWhere != null)
                {
                    query = query.Where(searchWhere);
                }
            }

            return query;
        }

        /// <inheritdoc />
        public Expression<Func<TEntity, bool>> Made<TEntity>(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return null;
            }

            search = string.Join("|",
                 search
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => $".*{x.ToLower().Trim()}.*"))
                .ToLower();

            var context = Expression.Parameter(typeof(TEntity));  // тип объекта, для которого нужно сделать поиск по полям
            var constant = Expression.Constant(search);  // текст, который мы ищем
            var expressions = new List<Expression>();

            CreateSearchExpression(
                typeof(TEntity),
                expressions,
                context);

            if (!expressions.Any())
            {
                Logger.LogWarning($"Для модели {typeof(TEntity).FullName} отсутствуют поля для поиска. " +
                                  $"(Ни одно строкое поле не помечено атрибутом [Searchable])");
                return null;
            }

            Func<object, object, string> concatFunc = string.Concat;
            var concatCall = expressions.Aggregate(
                (Expression f, Expression n) =>
                    Expression.Add(
                        f, n, concatFunc.Method));

            Func<string, string, RegexOptions, bool> match = Regex.IsMatch;
            var matchCall = Expression.Call(
                match.Method,
                concatCall,
                constant,
                Expression.Constant(RegexOptions.IgnoreCase));

            return expressions.Any()
                ? Expression.Lambda<Func<TEntity, bool>>(matchCall, context)
                : null;
        }

        private void CreateSearchExpression(
            Type type,
            List<Expression> expressions,
            Expression property)
        {
            // поиск производится только, по полям, которые помечены [SearchableAttribute]
            var props = type.GetProperties()
                .Where(x => x.IsDefined(typeof(SearchableAttribute)))
                .ToList();

            foreach (var prop in props)
            {
                if (prop.PropertyType == typeof(string))
                {
                    var typeName = prop.Name;
                    var p = Expression.Property(property, typeName);

                    expressions.Add(
                        Expression.Condition(
                            Expression.Equal(p, Expression.Constant(null, typeof(string))),
                            Expression.Constant(string.Empty, typeof(string)),
                            p));
                }
                else
                {
                    CreateSearchExpression(
                        prop.PropertyType,
                        expressions,
                        Expression.Property(property, prop));
                }
            }
        }
    }
}