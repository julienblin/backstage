namespace Backstage
{
    using System;

    /// <summary>
    /// The ContextProvider interface.
    /// A context provider provides the underlying persistence mechanism for some entities through contexts.
    /// </summary>
    public interface IContextProvider : IDisposable
    {
        /// <summary>
        /// Adds an entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        void Add(IEntity entity);

        /// <summary>
        /// Removes an entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        void Remove(IEntity entity);

        /// <summary>
        /// Reloads the <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        void Reload(IEntity entity);

        /// <summary>
        /// Gets an entity by its id.
        /// </summary>
        /// <param name="entityType">
        /// The type of entity.
        /// </param>
        /// <param name="id">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The entity.
        /// </returns>
        object GetById(Type entityType, object id);

        /// <summary>
        /// Fulfill a <see cref="IQuery{T}"/>.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <typeparam name="T">
        /// The type or returned result query.
        /// </typeparam>
        /// <returns>
        /// The result.
        /// </returns>
        T Fulfill<T>(IQuery<T> query);
    }
}
