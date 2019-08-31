using JetBrains.Annotations;

namespace Storage.Abstractions.Interfaces
{
    /// <summary>
    ///     Интерфейс-маркер сущности с шаблоном первичного ключа
    /// </summary>
    [PublicAPI]
    public interface IEntity
    {
        /// <summary> Первичный ключ в базе данных </summary>
        long Id { get; set; }
    }
}