namespace Backstage
{
    /// <summary>
    /// Allows the handling of <see cref="IDomainEvent"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of domain events to handle.
    /// </typeparam>
    public interface IHandleDomainEvent<in T>
        where T : IDomainEvent
    {
        /// <summary>
        /// Handles the <paramref name="domainEvent"/>.
        /// </summary>
        /// <param name="domainEvent">
        /// The event.
        /// </param>
        void Handle(T domainEvent);
    }
}
