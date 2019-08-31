using Newtonsoft.Json;
using Schedule.Entities.Abstract;
using Storage.Abstractions.Interfaces;

namespace Schedule.Models
{
    /// <summary> Модель для получения конкретного элемента из БД </summary>
    public class ItemRequest
    {
        /// <summary> Идентификатор объекта </summary>
        public long Id { get; set; }

        /// <summary> Вернуть удаленнный объект </summary>
        public bool ReturnDeleted { get; set; } = false;

        /// <summary> Вернуть удаленнных детей </summary>
        public bool ReturnDeletedChildren { get; set; } = true;

        /// <summary> Бросить исключение если объект не найден </summary>
        [JsonIgnore] public bool ThrowIfNull { get; set; }

        /// <summary> Конструктор, который проставляет идентификтаор по IEntity</summary>
        /// <param name="entity">Entity.</param>
        /// <param name="returnDeletedChildren">If set to <c>true</c> return deleted children.</param>
        /// <param name="returnDeleted">If set to <c>true</c> return deleted.</param>
        /// <param name="throwIfNull">If set to <c>true</c> throw if null.</param>
        public ItemRequest(IEntity entity, bool returnDeletedChildren = false, bool returnDeleted = false, bool throwIfNull = true)
            : this(returnDeletedChildren, returnDeleted, throwIfNull)
        {
            Id = entity.Id;
        }

        /// <summary> Конструктор, который проставляет идентификтаор по id</summary>
        /// <param name="id">Identifier.</param>
        /// <param name="returnDeletedChildren">If set to <c>true</c> return deleted children.</param>
        /// <param name="returnDeleted">If set to <c>true</c> return deleted.</param>
        /// <param name="throwIfNull">If set to <c>true</c> throw if null.</param>
        public ItemRequest(long id, bool returnDeletedChildren = false, bool returnDeleted = false, bool throwIfNull = true)
            : this(returnDeletedChildren, returnDeleted, throwIfNull)
        {
            Id = id;
        }


        /// <summary>Конструктор помощник</summary>
        /// <param name="returnDeletedChildren">If set to <c>true</c> return deleted children.</param>
        /// <param name="returnDeleted">If set to <c>true</c> return deleted.</param>
        /// <param name="throwIfNull">If set to <c>true</c> throw if null.</param>
        private ItemRequest(bool returnDeletedChildren, bool returnDeleted, bool throwIfNull)
            : this()
        {
            ReturnDeletedChildren = returnDeletedChildren;
            ReturnDeleted = returnDeleted;
            ThrowIfNull = throwIfNull;
        }

        /// <summary>Конструктор по умолчанию</summary>
        public ItemRequest()
        {
            ThrowIfNull = true;
        }

        /// <summary> Каст объекта типа Entity к типу ItemRequest </summary>
        /// <param name="entity"> Сущность типа Entity </param>
        public static implicit operator ItemRequest(Entity entity)
        {
            return entity == null
                ? null
                : new ItemRequest { Id = entity.Id };
        }

        /// <summary> Каст идентификатора сущности к типу ItemRequest. IsDeleted = false </summary>
        /// <param name="id">Идентификатор сущности.</param>
        public static implicit operator ItemRequest(long id)
        {
            return new ItemRequest(id);
        }
    }
}