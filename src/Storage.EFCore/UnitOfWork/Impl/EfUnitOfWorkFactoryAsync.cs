using System.Data;
using Storage.Abstractions.UnitOfWork;
using Storage.EFCore.CreateTransactionFacade;

namespace Storage.EFCore.UnitOfWork.Impl
{
    /// <summary>
    /// Фабрика Unit-of-work для EntityFramework
    /// </summary>
    public class EfUnitOfWorkFactoryAsync : IUnitOfWorkFactoryAsync
    {
        private readonly DataContext dbContext;
        private readonly ICreateTransactionFacade createTransactionFacade;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="dbContext">Контекст базы данных</param>
        /// <param name="transactionFacade">Фасад для создания транзакций</param>
        public EfUnitOfWorkFactoryAsync(DataContext dbContext, ICreateTransactionFacade transactionFacade)
        {
            createTransactionFacade = transactionFacade;
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Создание unit-of-work
        /// </summary>
        /// <returns>Unit-of-work</returns>
        public IUnitOfWorkAsync Create()
        {
            return Create(IsolationLevel.Unspecified);
        }

        /// <summary>
        /// Создание unit-of-work
        /// </summary>
        /// <param name="isolationLevel">Уровень изоляции транзакции</param>
        /// <returns>Unit-of-work</returns>
        public IUnitOfWorkAsync Create(IsolationLevel isolationLevel)
        {
            return new EFUnitOfWorkAsync(dbContext, createTransactionFacade.CreateTransaction(isolationLevel));
        }
    }
}