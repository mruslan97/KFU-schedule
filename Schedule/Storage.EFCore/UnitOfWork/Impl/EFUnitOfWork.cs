using System;
using Microsoft.EntityFrameworkCore.Storage;
using Common.Exceptions;
using Storage.Abstractions.UnitOfWork;

namespace Storage.EFCore.UnitOfWork.Impl
{
    /// <summary>
    ///     Unit-of-Work для EntityFramework
    /// </summary>
    public class EFUnitOfWork : IUnitOfWork
    {
        internal readonly IDbContextTransaction Transaction;

        private readonly DataContext dbContext;

        /// <summary>
        ///     Конструктор класа
        /// </summary>
        /// <param name = "dbContext" > Контекст базы данных </param>
        /// <param name = "transaction" > Транзакция контекста базы данных </param>
        public EFUnitOfWork(DataContext dbContext, IDbContextTransaction transaction)
        {
            this.dbContext = dbContext;
            Transaction = transaction;
        }

        /// <inheritdoc cref="IUnitOfWork.Commit()"/>
        public void Commit()
        {
            try
            {
                dbContext.SaveChanges();
                Transaction.Commit();
            }
            catch (Exception e)
            {
                Transaction.Rollback();
                throw new ResponseException($"Ошибка сохранения в базу {e.InnerException.Message}", 500);
            }
        }

        public void Save()
        {
            try
            {
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                Transaction.Rollback();
                throw new ResponseException($"Ошибка сохранения в базу {e.InnerException.Message}", 500);
            }
        }

        /// <inheritdoc cref="IUnitOfWork.Rollback()"/>
        public void Rollback()
        {
            Transaction.Rollback();
        }

        /// <summary>
        ///     Освобождение объекта. Откатывает транзакцию, если она не завершена успешно
        /// </summary>
        public void Dispose()
        {
            Transaction.Dispose();
        }
    }
}