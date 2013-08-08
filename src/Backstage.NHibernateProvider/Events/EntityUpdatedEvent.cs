namespace Backstage.NHibernateProvider.Events
{
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// The entity updated event. Raised when NHibernate flushes.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity.
    /// </typeparam>
    public class EntityUpdatedEvent<T> : IDomainEvent
        where T : IEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityUpdatedEvent{T}"/> class.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="dirtyPropertyNames">
        /// The dirty property names.
        /// </param>
        internal EntityUpdatedEvent(T entity, IEnumerable<string> dirtyPropertyNames)
        {
            this.Entity = entity;
            this.DirtyPropertyNames = dirtyPropertyNames;
        }

        /// <summary>
        /// Gets the modified entity.
        /// </summary>
        public T Entity { get; private set; }

        /// <summary>
        /// Gets or sets the dirty property names.
        /// </summary>
        public IEnumerable<string> DirtyPropertyNames { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "EntityUpdatedEvent({0})", this.Entity);
        }
    }
}
