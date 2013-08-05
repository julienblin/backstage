namespace Backstage.NHibernateProvider
{
    using NHibernate;

    /// <summary>
    /// Base class for NHibernate queries written in raw SQL.
    /// </summary>
    /// <typeparam name="T">
    /// The type of result.
    /// </typeparam>
    public abstract class NHQuerySql<T> : IQuery<T>, INHQuery
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
        public object Fulfill(IContextProvider contextProvider, ISession session)
        {
            // We flush before executing a query to make sure the vision of the database is accurate.
            session.Flush();
            return this.BuildSqlQuery(contextProvider, session);
        }

        /// <summary>
        /// Must be overriden in child classes to actually create and execute the SQL query.
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
        protected abstract T BuildSqlQuery(IContextProvider contextProvider, ISession session);
    }
}
