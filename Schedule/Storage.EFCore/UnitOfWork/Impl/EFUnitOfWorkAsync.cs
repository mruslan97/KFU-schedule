using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Storage.Abstractions.UnitOfWork;

namespace Storage.EFCore.UnitOfWork.Impl
{
    /// <inheritdoc />
    /// <summary>
    /// Unit-of-Work для EntityFramework
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class EFUnitOfWorkAsync : IUnitOfWorkAsync
    {
        private readonly DataContext dbContext;
        internal readonly IDbContextTransaction Transaction;

        /// <summary>
        /// Конструктор класа
        /// </summary>
        /// <param name="dbContext">Контекст базы данных</param>
        /// <param name="transaction">Транзакция контекста базы данных</param>
        public EFUnitOfWorkAsync(DataContext dbContext, IDbContextTransaction transaction)
        {
            this.dbContext = dbContext;
            Transaction = transaction;
        }

        /// <inheritdoc cref="IUnitOfWork.Commit()"/>
        public async Task Commit(CancellationToken cancellationToken = default)
        {
            try
            {
                await dbContext.SaveChangesAsync(cancellationToken);
                Transaction.Commit();
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <inheritdoc cref="IUnitOfWork.Rollback()"/>
        public void Rollback()
        {
            Transaction.Rollback();
        }

        /// <inheritdoc />
        /// <summary>
        /// Освобождение объекта. Откатывает транзакцию, если она не завершена успешно
        /// </summary>
        public void Dispose()
        {
            Transaction.Dispose();
        }
    }
}