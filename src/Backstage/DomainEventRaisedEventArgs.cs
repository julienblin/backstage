namespace Backstage
{
    using System;

    /// <summary>
    /// The domain event raised event args.
    /// </summary>
    public class DomainEventRaisedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainEventRaisedEventArgs"/> class.
        /// </summary>
        /// <param name="domainEvent">
        /// The domain event.
        /// </param>
        public DomainEventRaisedEventArgs(IDomainEvent domainEvent)
        {
            this.DomainEvent = domainEvent;
        }

        /// <summary>
        /// Gets the domain event raised.
        /// </summary>
        public IDomainEvent DomainEvent { get; private set; }
    }
}
