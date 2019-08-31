using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Storage.EFCore.CreateTransactionFacade
{
    /// <summary>
    ///     Фасад для содания транзакции. Инкапсулирует создание транзакции для тестирования.
    /// </summary>
    public interface ICreateTransactionFacade
    {
        /// <summary>
        ///     Создает транзакцию
        /// </summary>
        /// <param name = "isolationLevel" > Уровень изоляции </param>
        /// <returns> Транзакция </returns>
        IDbContextTransaction CreateTransaction(IsolationLevel isolationLevel);
    }
}