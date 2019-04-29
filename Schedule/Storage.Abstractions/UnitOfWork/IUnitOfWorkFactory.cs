using System.Data;

namespace Storage.Abstractions.UnitOfWork
{
    /// <summary>
    ///     Фабрика unit-of-work. Создает объект unit of work
    /// </summary>
    public interface IUnitOfWorkFactory
    {
        /// <summary>
        ///     Создание unit-of-work
        /// </summary>
        /// <returns> unit-of-work </returns>
        IUnitOfWork Create();

        /// <summary>
        ///     Создание unit-of-work
        /// </summary>
        /// <param name = "isolationLevel" > Уровень изоляции </param>
        /// <returns> unit-of-work </returns>
        IUnitOfWork Create(IsolationLevel isolationLevel);
    }
}