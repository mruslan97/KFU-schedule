using System;
using System.Collections.Generic;
using System.Linq;
using Common.Exceptions;
using Microsoft.Extensions.Logging;
using Storage.Abstractions.Interfaces;
using Storage.Abstractions.Repository;
using  vm = Schedule.Models;

namespace Schedule.Services.Impl
{
    /// <inheritdoc cref="ITimespanRepository{TEntity}"/>
    /// <typeparam name="TEntity">Тип сущности</typeparam>
    public class TimespanRepository<TEntity> : ITimespanRepository<TEntity>
        where TEntity : class, IPersistent
    {
        /// <summary> Репозиторий. Не имеет никакой другой логики кроме выполнения операции CRUD. </summary>
        public IRepository<TEntity> Repository { get; set; }

        /// <summary> Логгер. </summary>
        public ILogger<TimespanRepository<TEntity>> Logger { get; set; }

        /// <summary> декораторы на запросы получения данных </summary>
        /// <value>The query fetch decorators.</value>
        public IEnumerable<IQueryFetchDecorator> QueryFetchDecorators { get; set; }

        /// <inheritdoc />
        public TEntity Get(vm.ItemRequest request, Func<IQueryable<TEntity>, IQueryable<TEntity>> joins = null)
        {
            var res = Repository.Get(request.Id, joins, request.ReturnDeleted, request.ReturnDeletedChildren);

            return request.ThrowIfNull
                ? res ?? throw new ResponseException($"{typeof(TEntity).Name} не найден", 404)
                : res;
        }

        /// <inheritdoc cref="IRepository{TEntity}.Get"/>
        TEntity IRepository<TEntity>.Get(long id, Func<IQueryable<TEntity>, IQueryable<TEntity>> queryExpression, bool returnDeleted, bool returnDeletedChildren)
        {
            return Repository.Get(id, queryExpression, returnDeleted, returnDeletedChildren);
        }

        /// <inheritdoc cref="IRepository{TEntity}.GetAll"/>
        IQueryable<TEntity> IRepository<TEntity>.GetAll(bool returnDeleted, bool returnDeletedChildren)
        {
            return Repository.GetAll(returnDeleted, returnDeletedChildren);
        }

        /// <inheritdoc />
        public IQueryable<TEntity> GetAll(vm.PageListRequest pageListModel)
        {
            var all = Repository.GetAll(pageListModel.ReturnDeleted, pageListModel.ReturnDeletedChildren);

            foreach (var decorator in QueryFetchDecorators)
            {
                all = decorator.Decorate(all, pageListModel);
            }

            return all;
        }

        /// <inheritdoc cref="IRepository{TEntity}.Add"/>
        /// <summary>
        ///    В поле DateCreate фиксируется Дата создания
        /// </summary>
        public TEntity Add(TEntity entity)
        {
            entity.Created = DateTime.UtcNow;
            return Repository.Add(entity);
        }

        public IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
        {
            var createdTime = DateTime.Now;
            foreach (var entity in entities)
            {
                entity.Created = createdTime;
            }

            Repository.AddRange(entities);
            return entities;
        }

        /// <inheritdoc cref="IRepository{TEntity}.Update"/>
        /// <summary>
        ///    В поле DateChange фиксируется Дата изменения
        /// </summary>
        public TEntity Update(TEntity entity)
        {
            if (typeof(IDeletablePersistent).IsAssignableFrom(typeof(TEntity)))
            {
                var dal = Repository.GetAll()
                    .Select(x => new
                    {
                        ((IDeletablePersistent)x).IsDeleted,
                        ((IDeletablePersistent)x).Deleted,
                        x.Created,
                        x.Id
                    })
                    .First(x => x.Id == entity.Id);

                entity.Created = dal.Created;

                var deletableEntity = (IDeletablePersistent)entity;

                if (deletableEntity.IsDeleted && !dal.IsDeleted)
                {
                    deletableEntity.Deleted = DateTime.UtcNow;
                }
                else if (!deletableEntity.IsDeleted)
                {
                    deletableEntity.Deleted = null;
                }
                else
                {
                    deletableEntity.Deleted = dal.Deleted;
                }
            }
            else
            {
                var dal = Repository.GetAll()
                    .Select(x => new
                    {
                        x.Created,
                        x.Id
                    })
                    .First(x => x.Id == entity.Id);
                entity.Created = dal.Created;
            }

            entity.Updated = DateTime.UtcNow;

            return Repository.Update(entity);
        }

        /// <inheritdoc cref="IRepository{TEntity}.Delete(long)"/>
        /// <summary>
        ///     Если {TEntity} является <see cref="IDeletablePersistent"/>, то фиксируется дата
        ///     (<see cref="IDeletablePersistent.Deleted"/>) пометки об
        ///     удалении и выставляется полe <see cref="IDeletablePersistent.IsDeleted"/> <code>true</code>
        /// </summary>
        public TEntity Delete(long id)
        {
            if (typeof(IDeletablePersistent).IsAssignableFrom(typeof(TEntity)))
            {
                var entity = Repository.Get(id);

                var deletable = (IDeletablePersistent)entity;

                deletable.IsDeleted = true;
                deletable.Deleted = DateTime.UtcNow;

                return entity;
            }

            return Repository.Delete(id);
        }

        /// <inheritdoc cref="IRepository{TEntity}.Delete(TEntity)"/>
        /// <summary>
        ///     Если {TEntity} является <see cref="IDeletablePersistent"/>, то фиксируется дата
        ///     (<see cref="IDeletablePersistent.Deleted"/>) пометки об
        ///     удалении и выставляется полe <see cref="IDeletablePersistent.IsDeleted"/> <code>true</code>
        /// </summary>
        public TEntity Delete(TEntity entity)
        {
            return Delete(entity.Id);
        }

        public IQueryable<TEntity> DeleteRange(IQueryable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Delete(entity);
            }

            return entities;
        }
    }
}