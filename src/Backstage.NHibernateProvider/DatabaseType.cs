namespace Backstage.NHibernateProvider
{
    /// <summary>
    /// The database type.
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>
        /// SQLite database.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sq", Justification = "SQLite is a product.")]
        SQLite,

        /// <summary>
        /// SQL server 2008 database.
        /// </summary>
        SqlServer2008
    }
}
