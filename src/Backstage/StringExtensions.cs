namespace Backstage
{
    using System.Globalization;

    /// <summary>
    /// Extension methods for <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Replaces each format item in a specified string with the text equivalent of a corresponding object's value.
        /// </summary>
        /// <param name="formatMessage">
        /// A composite format string.
        /// </param>
        /// <param name="args">
        /// An object array that contains zero or more objects to format.
        /// </param>
        /// <returns>
        /// A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.
        /// </returns>
        public static string Format(this string formatMessage, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, formatMessage, args);
        }
    }
}
