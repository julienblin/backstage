namespace Backstage.NHibernateProvider
{
    using NHibernate;

    /// <summary>
    /// Base class for NHibernate queries based on <see cref="IQueryOver{TRoot,TSubtype}"/> API.
    /// </summary>
    /// <typeparam name="T">
    /// The type of result.
    /// </typeparam>
    public abstract class NHQueryQueryOver<T> : IQuery<IPreparedQuery<T>>, INHQuery
    {
        /// <summary>
        /// Fulfills the query - call by the NHibernate <see cref="IContextProvider"/>.
        /// </summary>
        /// <param name="contextProvider">
        /// The context provider.
        /// </param>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <returns>
        /// The result.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Clarifies the public interface.")]
        object INHQuery.Fulfill(IContextProvider contextProvider, ISession session)
        {
            return this.BuildPreparedQuery(contextProvider, session);
        }

        /// <summary>
        /// Builds the <see cref="IPreparedQuery{T}"/> by calling <see cref="BuildQueryOver"/>
        /// </summary>
        /// <param name="contextProvider">
        /// The context provider.
        /// </param>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <returns>
        /// The <see cref="IPreparedQuery{T}"/>.
        /// </returns>
        protected virtual IPreparedQuery<T> BuildPreparedQuery(IContextProvider contextProvider, ISession session)
        {
            return new PreparedQueryQueryOver<T>(session, this.BuildQueryOver(contextProvider, session));
        }

        /// <summary>
        /// Must be implemented to create the <see cref="IQueryOver{TRoot,TSubtype}"/> that will
        /// be used by the final <see cref="IPreparedQuery{T}"/>.
        /// </summary>
        /// <param name="contextProvider">
        /// The context provider.
        /// </param>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <returns>
        /// The <see cref="IQueryOver"/>.
        /// </returns>
        protected abstract IQueryOver<T, T> BuildQueryOver(IContextProvider contextProvider, ISession session);
    }
}
