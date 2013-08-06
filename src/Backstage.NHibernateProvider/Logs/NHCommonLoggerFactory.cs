namespace Backstage.NHibernateProvider.Logs
{
    using System;

    using NHibernate;

    /// <summary>
    /// NHibernate <see cref="ILoggerFactory"/> for <see cref="Common.Logging"/>.
    /// </summary>
    public class NHCommonLoggerFactory : ILoggerFactory
    {
        /// <summary>
        /// Returns the logger for <paramref name="keyName"/>.
        /// </summary>
        /// <param name="keyName">
        /// The key name.
        /// </param>
        /// <returns>
        /// The <see cref="IInternalLogger"/>.
        /// </returns>
        public IInternalLogger LoggerFor(string keyName)
        {
            return new NHCommonInternalLogger(keyName);
        }

        /// <summary>
        /// Returns the logger for <paramref name="type"/>.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="IInternalLogger"/>.
        /// </returns>
        public IInternalLogger LoggerFor(Type type)
        {
            return new NHCommonInternalLogger(type);
        }
    }
}
