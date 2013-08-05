namespace Backstage
{
    /// <summary>
    /// The order type.
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// Ascendant order.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Asc", Justification = "SQL Abbreviation")]
        Asc,

        /// <summary>
        /// Descendant order.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Desc", Justification = "SQL Abbreviation")]
        Desc
    }
}
