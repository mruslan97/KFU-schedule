﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Storage.Abstractions.Interfaces;
using Storage.Abstractions.Repository;

namespace Storage.EFCore.Repository.Impl
{
    /// <summary>
    ///     Базовый репозиторий сущностей. Оперирует сущностями в базе данных.
    /// </summary>
    /// <typeparam name = "TEntity" > Тип, который мапится в таблицу из БД </typeparam>
    public class EFRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        private readonly DataContext currentDbContext;

        /// <summary>
        ///     Конструктор класса
        /// </summary>
        /// <param name = "currentDbContext" > Текущий контекст базы данных </param>
        public EFRepository(DataContext currentDbContext)
        {
            this.currentDbContext = currentDbContext;
        }

        [CanBeNull]
        public TEntity Get(long id,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> queryExpression = null,
            bool returnDeleted = false,
            bool returnDeletedChildren = false)
        {
            var setEntity = (IQueryable<TEntity>)currentDbContext.Set<TEntity>();

            setEntity = SetUndeletable(setEntity, returnDeleted, returnDeletedChildren);

            if (queryExpression != null)
            {
                setEntity = queryExpression(setEntity);
            }

            return setEntity.FirstOrDefault(entity => entity.Id == id);
        }

        public IQueryable<TEntity> GetAll(
            bool returnDeleted = false,
            bool returnDeletedChildren = false)
        {
            var setEntity = (IQueryable<TEntity>)currentDbContext.Set<TEntity>();

            setEntity = SetUndeletable(setEntity, returnDeleted, returnDeletedChildren);

            return setEntity;
        }

        public TEntity Add(TEntity entity)
        {
            currentDbContext.Add(entity);

            return entity;
        }

        public IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
        {
            currentDbContext.AddRange(entities);

            return entities;
        }

        public TEntity Update(TEntity entity)
        {
            currentDbContext.Update(entity);

            return entity;
        }

        public TEntity Delete(long id)
        {
            var entity = Get(id);

            if (entity != null)
            {
                Delete(entity);
            }

            return entity;
        }

        public TEntity Delete(TEntity entity)
        {
            currentDbContext.Set<TEntity>()
                .Remove(entity);

            return entity;
        }

        public IQueryable<TEntity> DeleteRange(IQueryable<TEntity> entities)
        {
            currentDbContext.Set<TEntity>().RemoveRange(entities);

            return entities;
        }

        public IQueryable<TEntity> SetUndeletable(
            IQueryable<TEntity> setEntity,
            bool returnDeleted,
            bool returnDeletedChildren)
        {
            if (typeof(IDeletablePersistent).IsAssignableFrom(typeof(TEntity)))
            {
                if (returnDeleted)
                {
                    setEntity = setEntity.IgnoreQueryFilters();
                }

                if (!returnDeleted && returnDeletedChildren)
                {
                    setEntity = setEntity
                        .IgnoreQueryFilters()
                        .Where(x => ((IDeletablePersistent)x).IsDeleted == false);
                }
            }

            return setEntity;
        }
    }
}