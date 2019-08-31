using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Storage.Abstractions.SqlQueryExecutor
{
    /// <summary>
    ///     Утилита для SQL-запросов
    /// </summary>
    public interface IQueryExecutor
    {
        /// <summary>
        ///     Выполнение SQL-запроса
        /// </summary>
        /// <param name = "sql" > SQL-запрос </param>
        /// <param name = "param" > Параметры запроса </param>
        /// <returns> Результат выполнения SQL-запроса </returns>
        IEnumerable<T> Execute<T>(string sql, object param);

        /// <summary>
        ///     Выполнение SQL-команды
        /// </summary>
        /// <param name = "sql" > SQL-запрос </param>
        /// <param name = "param" > Параметры запроса </param>
        /// <returns> Результат выполнения SQL-запроса </returns>
        int ExecuteSqlCommand(string sql, object param);

        /// <summary>
        ///     Асинхронное выполнение SQL-запроса
        /// </summary>
        /// <param name = "sql" > SQL-запрос </param>
        /// <param name = "param" > Параметры запроса </param>
        /// <param name = "cancellationToken" > Токен отмены </param>
        /// <returns> Результат выполнения SQL-запроса </returns>
        Task<IEnumerable<T>> ExecuteAsync<T>(string sql, object param,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Асинхронное выполнение SQL-команды
        /// </summary>
        /// <param name = "sql" > SQL-запрос </param>
        /// <param name = "param" > Параметры запроса </param>
        /// <param name = "cancellationToken" > Токен отмены </param>
        /// <returns> Результат выполнения SQL-запроса </returns>
        Task<int> ExecuteSqlCommandAsync(string sql, object param, CancellationToken cancellationToken);
    }
}