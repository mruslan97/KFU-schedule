using Storage.Abstractions.Interfaces;

namespace Schedule.Entities.Abstract
{
    /// <summary>
    ///     Все сущности в БД имеют дату создания и дату изменения.
    ///     DateCreate меняется только при добавлениии.
    ///     DateChange меняется только при изменении.
    /// 
    ///     При наследовании использовать тип <see cref="Persistent"/> 
    ///  
    ///     Для типов, которые не удаляются, а помечаются удаленным необъодимо использовать
    ///     <see cref="IDeletablePersistent"/>
    /// </summary>
    public abstract class Entity : IEntity
    {
        /// <summary>
        ///       Первичный ключ в базе данных  
        /// </summary>
        public long Id { get; set; }

        /// <summary> Implicitly cast entity to id. </summary>
        /// <returns> Entity identifier.</returns>
        /// <param name="entity">Entity.</param>
        public static implicit operator long(Entity entity)
        {
            return entity.Id;
        }
    }
}