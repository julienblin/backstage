namespace Backstage.NHibernateProvider
{
    /// <summary>
    /// Base class for NHibernate-managed entities.
    /// </summary>
    /// <typeparam name="T">
    /// The type of ids.
    /// </typeparam>
    public abstract class NHEntity<T> : IEntity
    {
#pragma warning disable 649
        /// <summary>
        /// The id.
        /// </summary>
        private T id;
#pragma warning restore 649

        /// <summary>
        /// Gets the id.
        /// </summary>
        public virtual T Id
        {
            get { return this.id; }
        }
    }
}
