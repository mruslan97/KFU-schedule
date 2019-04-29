using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Storage.EFCore.CreateTransactionFacade.Impl
{
    /// <summary>
    ///     Фасад для содания транзакции
    /// </summary>
    public class CreateTransactionFacade : ICreateTransactionFacade
    {
        private readonly DataContext dbContext;

        /// <summary>
        ///     Конструктор класса
        /// </summary>
        /// <param name = "dbContext" > Контекст базы данных </param>
        public CreateTransactionFacade(DataContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        ///     Создает транзакцию
        /// </summary>
        /// <param name = "isolationLevel" > Уровень изоляции </param>
        /// <returns> Транзакция </returns>
        public IDbContextTransaction CreateTransaction(IsolationLevel isolationLevel)
        {
            return dbContext.Database.BeginTransaction(isolationLevel);
        }
    }
}