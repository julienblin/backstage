namespace Backstage
{
    using System.Linq;

    /// <summary>
    /// Utilities for computing unique hash codes based on values.
    /// </summary>
    public static class HashCode
    {
        /// <summary>
        /// Generates the hash code based on values.
        /// </summary>
        /// <param name="values">
        /// The list of values to consider.
        /// </param>
        /// <returns>
        /// The hash code.
        /// </returns>
        public static int Generate(params object[] values)
        {
            return values.Where(value => value != null).Aggregate(27, (current, value) => (13 * current) + value.GetHashCode());
        }
    }
}
