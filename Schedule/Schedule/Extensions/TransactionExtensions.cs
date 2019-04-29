using System;
using System.Threading.Tasks;
using Storage.Abstractions.UnitOfWork;

namespace Schedule.Extensions
{
    /// <summary> метод расширения для транзакций </summary>
    public static class TransactionExtensions
    {
        /// <summary> нужно ли делать транхзакции </summary>
        /// <param name="uowFactory">Uow factory.</param>
        /// <param name="action">Action.</param>
        /// <param name="useTransaction">If set to <c>true</c> use transaction.</param>
        public static void Transaction(this IUnitOfWorkFactory uowFactory, Action action, bool useTransaction = true)
        {
            if (useTransaction)
            {
                using (var uow = uowFactory.Create())
                {
                    action();
                    uow.Commit();
                }
            }
            else
            {
                action();
            }
        }

        /// <summary> нужно ли делать транхзакции </summary>
        /// <param name="uowFactory">Uow factory.</param>
        /// <param name="action">Action.</param>
        /// <param name="useTransaction">If set to <c>true</c> use transaction.</param>
        public static async Task Transaction(this IUnitOfWorkFactory uowFactory, Func<Task> action, bool useTransaction = true)
        {
            if (useTransaction)
            {
                using (var uow = uowFactory.Create())
                {
                    await action();
                    uow.Commit();
                }
            }
            else
            {
                await action();
            }
        }

        /// <summary> Вызвать комит если требуется </summary>
        /// <param name="uow">Uow.</param>
        /// <param name="useTransaction">If set to <c>true</c> use transaction.</param>
        public static void Commit(this IUnitOfWork uow, bool useTransaction)
        {
            if (useTransaction)
            {
                uow.Commit();
            }
        }
    }
}