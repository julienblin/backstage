namespace Backstage.NHibernateProvider
{
    using NHibernate;

    /// <summary>
    /// Base class for NHibernate queries based on <see cref="ICriteria"/> API.
    /// </summary>
    /// <typeparam name="T">
    /// The type of result.
    /// </typeparam>
    public abstract class NHQueryCriteria<T> : IQuery<IPreparedQuery<T>>, INHQuery
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
        /// Builds the <see cref="IPreparedQuery{T}"/> by calling <see cref="BuildCriteria"/>
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
            return new PreparedQueryCriteria<T>(session, this.BuildCriteria(contextProvider, session));
        }

        /// <summary>
        /// Must be implemented to create the <see cref="ICriteria"/> that will
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
        protected abstract ICriteria BuildCriteria(IContextProvider contextProvider, ISession session);
    }
}
