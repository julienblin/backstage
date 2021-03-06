﻿namespace Backstage.NHibernateProvider.Events
{
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// The entity inserted event.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity.
    /// </typeparam>
    public class EntityInsertedEvent<T> : IDomainEvent
        where T : IEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityInsertedEvent{T}"/> class.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        internal EntityInsertedEvent(T entity)
        {
            this.Entity = entity;
        }

        /// <summary>
        /// Gets the modified entity.
        /// </summary>
        public T Entity { get; private set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "EntityInsertedEvent({0})", this.Entity);
        }
    }
}
