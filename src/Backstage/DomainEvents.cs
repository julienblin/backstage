namespace Backstage
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Common.Logging;

    /// <summary>
    /// <see cref="IDomainEvent"/> management.
    /// </summary>
    public static class DomainEvents
    {
        /// <summary>
        /// The mangled name for <see cref="IHandleDomainEvent{T}"/>, for interface search.
        /// </summary>
        internal static readonly string HandleDomainEventsMangledName = typeof(IHandleDomainEvent<>).Name;

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The sync root.
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// The global subscriptions.
        /// </summary>
        private static readonly IDictionary<Type, IList<Type>> Subscriptions = new Dictionary<Type, IList<Type>>();

        /// <summary>
        /// Gets a copy of the subscriptions.
        /// </summary>
        /// <returns>
        /// The subscriptions.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "OK here - this is a copy.")]
        public static IDictionary<Type, IList<Type>> GetSubscriptions()
        {
            lock (SyncRoot)
            {
                return new ReadOnlyDictionary<Type, IList<Type>>(Subscriptions);
            }
        }

        /// <summary>
        /// Raises the <paramref name="domainEvent"/> synchronously.
        /// </summary>
        /// <param name="domainEvent">
        /// The domain event.
        /// </param>
        /// <typeparam name="T">
        /// The type of domain event.
        /// </typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Ok for domain events.")]
        public static void Raise<T>(T domainEvent)
            where T : class, IDomainEvent
        {
            domainEvent.ThrowIfNull("domainEvent");
            lock (SyncRoot)
            {
                if (!Subscriptions.ContainsKey(domainEvent.GetType()))
                {
                    return;
                }

                foreach (var subscriptionType in Subscriptions[domainEvent.GetType()])
                {
                    IHandleDomainEvent<T> subscription = null;
                    try
                    {
                        subscription = (IHandleDomainEvent<T>)Activator.CreateInstance(subscriptionType);
                        subscription.Handle(domainEvent);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(Resources.ErrorWhileRaisingDomainEvent.Format(domainEvent, subscriptionType), ex);
                        throw new BackstageException(
                            Resources.ErrorWhileRaisingDomainEvent.Format(domainEvent, subscriptionType), ex);
                    }
                    finally
                    {
// ReSharper disable SuspiciousTypeConversion.Global
                        var disposableSubscription = subscription as IDisposable;
// ReSharper restore SuspiciousTypeConversion.Global
                        if (disposableSubscription != null)
                        {
                            disposableSubscription.Dispose();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Subscribes to a domain event.
        /// </summary>
        /// <typeparam name="T">
        /// The handler type.
        /// </typeparam>
        /// <typeparam name="TEvent">
        /// The event type.
        /// </typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "This is the case.")]
        public static void Subscribe<T, TEvent>()
            where T : IHandleDomainEvent<TEvent>
            where TEvent : IDomainEvent
        {
            Subscribe(typeof(T));
        }

        /// <summary>
        /// Subscribes to a domain event.
        /// </summary>
        /// <param name="handlerType">
        /// The handler type. Must implement <see cref="IHandleDomainEvent{T}"/>.
        /// </param>
        public static void Subscribe(Type handlerType)
        {
            handlerType.ThrowIfNull("handlerType");
            var domainEventType = GetDomainEventType(handlerType);

            lock (SyncRoot)
            {
                if (!Subscriptions.ContainsKey(domainEventType))
                {
                    Subscriptions[domainEventType] = new List<Type>();
                }

                if (!Subscriptions[domainEventType].Contains(handlerType))
                {
                    Subscriptions[domainEventType].Add(handlerType);
                }
            }
        }

        /// <summary>
        /// Unsubscribes to a domain event.
        /// </summary>
        /// <typeparam name="T">
        /// The handler type.
        /// </typeparam>
        /// <typeparam name="TEvent">
        /// The event type.
        /// </typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "This is the case.")]
        public static void Unsubscribe<T, TEvent>()
            where T : IHandleDomainEvent<TEvent>
            where TEvent : IDomainEvent
        {
            Unsubscribe(typeof(T));
        }

        /// <summary>
        /// Unsubscribes to domain events.
        /// </summary>
        /// <param name="handlerType">
        /// The handler type.
        /// </param>
        public static void Unsubscribe(Type handlerType)
        {
            handlerType.ThrowIfNull("handlerType");
            var domainEventType = GetDomainEventType(handlerType);

            lock (SyncRoot)
            {
                if (!Subscriptions.ContainsKey(domainEventType))
                {
                    return;
                }

                Subscriptions[domainEventType].Remove(handlerType);
                if (Subscriptions[domainEventType].Count == 0)
                {
                    Subscriptions.Remove(domainEventType);
                }
            }
        }

        /// <summary>
        /// Returns the type of domain event, given the handler type.
        /// </summary>
        /// <param name="handlerType">
        /// The handler type.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/> of the domain event.
        /// </returns>
        /// <exception cref="BackstageException">
        /// If the type doesn't implement <see cref="IHandleDomainEvent{T}"/>.
        /// </exception>
        private static Type GetDomainEventType(Type handlerType)
        {
            var baseInterfaceType = handlerType.GetInterface(HandleDomainEventsMangledName);
            if (baseInterfaceType == null)
            {
                throw new BackstageException(Resources.DomainHandlersShouldImplementIHandleDomainEvent.Format(handlerType));
            }

            var domainEventType = baseInterfaceType.GetGenericArguments()[0];
            return domainEventType;
        }
    }
}
