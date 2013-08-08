namespace Backstage.NHibernateProvider.Events
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    using NHibernate.Event;

    /// <summary>
    /// Raises domain events.
    /// </summary>
    internal class DomainEventsEventListener : IPostInsertEventListener, IPostUpdateEventListener, IPostDeleteEventListener
    {
        /// <summary>
        /// The on post insert.
        /// </summary>
        /// <param name="event">
        /// The event.
        /// </param>
        public void OnPostInsert(PostInsertEvent @event)
        {
            if (!(@event.Entity is IEntity))
            {
                return;
            }

            var domainEvent = CreateDomainEvent(typeof(EntityInsertedEvent<>), @event.Entity);
            DomainEvents.Raise(domainEvent);
        }

        /// <summary>
        /// The on post update.
        /// </summary>
        /// <param name="event">
        /// The event.
        /// </param>
        public void OnPostUpdate(PostUpdateEvent @event)
        {
            if (!(@event.Entity is IEntity))
            {
                return;
            }

            var updatedPropertyNames = new List<string>();

            for (var i = 0; i < @event.OldState.Length; ++i)
            {
                if (object.Equals(@event.OldState[i], @event.State[i]))
                {
                    continue;
                }

                updatedPropertyNames.Add(@event.Persister.PropertyNames[i]);
            }

            var domainEvent = CreateDomainEvent(typeof(EntityUpdatedEvent<>), @event.Entity, (IEnumerable<string>)updatedPropertyNames.AsReadOnly());
            DomainEvents.Raise(domainEvent);
        }

        /// <summary>
        /// The on post delete.
        /// </summary>
        /// <param name="event">
        /// The event.
        /// </param>
        public void OnPostDelete(PostDeleteEvent @event)
        {
            if (!(@event.Entity is IEntity))
            {
                return;
            }

            var domainEvent = CreateDomainEvent(typeof(EntityDeletedEvent<>), @event.Entity);
            DomainEvents.Raise(domainEvent);
        }

        /// <summary>
        /// Creates a domain event, based on a generic type (e.g. <see cref="EntityUpdatedEvent{T}"/>).
        /// </summary>
        /// <param name="domainEventBaseType">
        /// The domain event base type.
        /// </param>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="additionalArgs">
        /// The additional arguments to pass to the constructor (after the entity).
        /// </param>
        /// <returns>
        /// The <see cref="IDomainEvent"/>.
        /// </returns>
        private static IDomainEvent CreateDomainEvent(Type domainEventBaseType, object entity, params object[] additionalArgs)
        {
            var constructorArgs = new ArrayList { entity };
            if (additionalArgs != null)
            {
                constructorArgs.AddRange(additionalArgs);
            }

            return (IDomainEvent)Activator.CreateInstance(
                    domainEventBaseType.MakeGenericType(entity.GetType()),
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    constructorArgs.ToArray(),
                    null);
        }
    }
}
