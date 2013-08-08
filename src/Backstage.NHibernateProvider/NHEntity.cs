namespace Backstage.NHibernateProvider
{
    using System.Globalization;

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

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}({1})", this.GetType().Name, this.Id);
        }
    }
}
