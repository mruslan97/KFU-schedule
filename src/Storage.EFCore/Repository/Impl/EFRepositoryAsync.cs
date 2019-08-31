using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Storage.Abstractions.Interfaces;
using Storage.Abstractions.Repository;

namespace Storage.EFCore.Repository.Impl
{
    /// <summary>
    /// Базовый репозиторий сущностей. Оперирует сущностями в базе данных.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    // ReSharper disable once InconsistentNaming
    public class EFRepositoryAsync<TEntity> : IRepositoryAsync<TEntity>
        where TEntity : class, IEntity
    {
        private readonly ICurrentDbContext currentDbContext;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="currentDbContext">Текущий контекст базы данных</param>
        public EFRepositoryAsync(ICurrentDbContext currentDbContext)
        {
            this.currentDbContext = currentDbContext;
        }


        public async Task<TEntity> Get(long id, CancellationToken cancellationToken = default)
        {
            var parameterExpr = Expression.Parameter(typeof(TEntity));
            var idPropExpr = Expression.Property(parameterExpr, nameof(IEntity.Id));
            var idExpr = Expression.Constant(id);
            var eqExpr = Expression.Equal(idPropExpr, idExpr);
            var expr = Expression.Lambda<Func<TEntity, bool>>(eqExpr, parameterExpr);

            return await GetAll()
                .FirstOrDefaultAsync(expr, cancellationToken);
        }


        public IQueryable<TEntity> GetAll()
        {
            //if (returnDeleted == false && typeof(IDeletableObject).IsAssignableFrom(typeof(TEntity)))
            //{
            //    return this.currentDbContext.Context.Set<TEntity>().Where(x => ((IDeletableObject)x).Deleted == false);
            //}

            return currentDbContext.Context.Set<TEntity>();
        }


        public async Task Delete(long id, CancellationToken token = default)
        {
            var entity = await Get(id, token)
                ?? throw new ArgumentException($"Сущность с {nameof(IEntity.Id)} = {id} не найдена");

            Delete(entity);
        }


        public void Delete(TEntity entity)
        {
            currentDbContext.Context.Set<TEntity>().Remove(entity);
        }


        public async Task Add(TEntity entity, CancellationToken token = default)
        {
            await currentDbContext.Context.AddAsync(entity, token);
        }
    }
}