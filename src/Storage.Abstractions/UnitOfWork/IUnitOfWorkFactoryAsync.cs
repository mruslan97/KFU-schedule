using System.Data;

namespace Storage.Abstractions.UnitOfWork
{
    /// <summary>
    /// Фабрика unit-of-work. Создает объект unit of work
    /// </summary>
    public interface IUnitOfWorkFactoryAsync
    {
        /// <summary>
        /// Создание unit-of-work
        /// </summary>
        /// <returns>unit-of-work</returns>
        IUnitOfWorkAsync Create();

        /// <summary>
        /// Создание unit-of-work
        /// </summary>
        /// <param name="isolationLevel">Уровень изоляции</param>
        /// <returns>unit-of-work</returns>
        IUnitOfWorkAsync Create(IsolationLevel isolationLevel);
    }
}