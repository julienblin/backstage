namespace Backstage
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading;

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
        /// The reader writer lock.
        /// </summary>
        private static readonly ReaderWriterLockSlim ReaderWriterLock = new ReaderWriterLockSlim();

        /// <summary>
        /// The handlers.
        /// </summary>
        private static readonly IDictionary<Type, ArrayList> Handlers = new Dictionary<Type, ArrayList>();

        /// <summary>
        /// The domain event raised.
        /// </summary>
        public static event EventHandler<DomainEventRaisedEventArgs> DomainEventRaised;

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
            ReaderWriterLock.EnterReadLock();
            Log.Debug(Resources.RaisingEvent.Format(domainEvent));
            try
            {
                OnDomainEventRaised(new DomainEventRaisedEventArgs(domainEvent));

                if (!Handlers.ContainsKey(typeof(T)))
                {
                    return;
                }

                foreach (var handler in Handlers[typeof(T)])
                {
                    try
                    {
                        ((IHandleDomainEvent<T>)handler).Handle(domainEvent);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(Resources.ErrorWhileRaisingDomainEvent.Format(domainEvent, handler), ex);
                        throw new BackstageException(Resources.ErrorWhileRaisingDomainEvent.Format(domainEvent, handler), ex);
                    }
                }

                Log.Debug(Resources.EventRaised.Format(domainEvent));
            }
            finally
            {
                ReaderWriterLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Get the handlers for the <typeparamref name="T"/> (type od <see cref="IDomainEvent"/>).
        /// </summary>
        /// <typeparam name="T">
        /// The type of <see cref="IDomainEvent"/>.
        /// </typeparam>
        /// <returns>
        /// The handlers.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Ok here.")]
        public static ArrayList GetHandlers<T>()
            where T : IDomainEvent
        {
            return GetHandlers(typeof(T));
        }

        /// <summary>
        /// Get the handlers for the <paramref name="domainEventType"/>.
        /// </summary>
        /// <param name="domainEventType">
        /// The <see cref="IDomainEvent"/> type.
        /// </param>
        /// <returns>
        /// The handlers.
        /// </returns>
        public static ArrayList GetHandlers(Type domainEventType)
        {
            ReaderWriterLock.EnterReadLock();
            try
            {
                return !Handlers.ContainsKey(domainEventType) ? new ArrayList() : new ArrayList(Handlers[domainEventType]);
            }
            finally
            {
                ReaderWriterLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Subscribe to an event.
        /// </summary>
        /// <param name="handler">
        /// The handler.
        /// </param>
        /// <typeparam name="T">
        /// The type of event.
        /// </typeparam>
        public static void Subscribe<T>(IHandleDomainEvent<T> handler)
            where T : IDomainEvent
        {
            Subscribe((object)handler);
        }

        /// <summary>
        /// Subscribe to an event. Non-generic version.
        /// </summary>
        /// <param name="handler">
        /// The handler. Must be an <see cref="IHandleDomainEvent{T}"/>.
        /// </param>
        public static void Subscribe(object handler)
        {
            handler.ThrowIfNull("handler");
            var domainEventType = GetDomainEventType(handler.GetType());
            ReaderWriterLock.EnterWriteLock();
            try
            {
                if (!Handlers.ContainsKey(domainEventType))
                {
                    Handlers[domainEventType] = new ArrayList();
                }

                if (!Handlers[domainEventType].Contains(handler))
                {
                    Handlers[domainEventType].Add(handler);
                }
            }
            finally
            {
                ReaderWriterLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Unsubscribe a handler.
        /// </summary>
        /// <param name="handler">
        /// The handler.
        /// </param>
        /// <typeparam name="T">
        /// The type of event.
        /// </typeparam>
        public static void Unsubscribe<T>(IHandleDomainEvent<T> handler)
            where T : IDomainEvent
        {
            Unsubscribe((object)handler);
        }

        /// <summary>
        /// Unsubscribe a handler.
        /// </summary>
        /// <param name="handler">
        /// The handler. Must be an <see cref="IHandleDomainEvent{T}"/>.
        /// </param>
        public static void Unsubscribe(object handler)
        {
            handler.ThrowIfNull("handler");
            var domainEventType = GetDomainEventType(handler.GetType());
            ReaderWriterLock.EnterWriteLock();
            try
            {
                if (!Handlers.ContainsKey(domainEventType))
                {
                    return;
                }

                if (Handlers[domainEventType].Contains(handler))
                {
                    Handlers[domainEventType].Remove(handler);
                }
            }
            finally
            {
                ReaderWriterLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Unsubscribe all handlers for an event.
        /// </summary>
        /// <typeparam name="T">
        /// The type of event.
        /// </typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Ok here.")]
        public static void Unsubscribe<T>()
            where T : IDomainEvent
        {
            ReaderWriterLock.EnterWriteLock();
            try
            {
                if (Handlers.ContainsKey(typeof(T)))
                {
                    Handlers.Remove(typeof(T));
                }
            }
            finally
            {
                ReaderWriterLock.ExitWriteLock();
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

        /// <summary>
        /// The on domain event raised.
        /// </summary>
        /// <param name="e">
        /// The arguments.
        /// </param>
        private static void OnDomainEventRaised(DomainEventRaisedEventArgs e)
        {
            var handler = DomainEventRaised;
            if (handler != null)
            {
                handler(null, e);
            }
        }
    }
}
