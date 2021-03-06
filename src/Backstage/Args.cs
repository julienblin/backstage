﻿namespace Backstage
{
    using System;

    /// <summary>
    /// Helper class for parameters validation.
    /// </summary>
    public static class Args
    {
        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if <paramref name="parameter"/> is null.
        /// </summary>
        /// <param name="parameter">
        /// The parameter to check for null.
        /// </param>
        /// <param name="parameterName">
        /// The parameter name.
        /// </param>
        /// <typeparam name="T">
        /// The parameter type.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="parameter"/> is null.
        /// </exception>
        public static void ThrowIfNull<T>(this T parameter, string parameterName)
            where T : class
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if <paramref name="parameter"/> is null or empty.
        /// </summary>
        /// <param name="parameter">
        /// The parameter to check for null or empty.
        /// </param>
        /// <param name="parameterName">
        /// The parameter name.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="parameter"/> is null.
        /// </exception>
        public static void ThrowIfNullOrEmpty(this string parameter, string parameterName)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                throw new ArgumentNullException(parameterName);
            }
        }
    }
}
