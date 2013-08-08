namespace Backstage
{
    using System;

    using Common.Logging;

    /// <summary>
    /// Convenience holder for the current <see cref="IContextFactory"/>.
    /// </summary>
    public static class ContextFactory
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The sync root.
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// The current context factory.
        /// </summary>
        private static IContextFactory currentContextFactory;

        /// <summary>
        /// Gets the current <see cref="IContextFactory"/>.
        /// </summary>
        /// <exception cref="BackstageException">
        /// If no current context factory has been started.
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Expected behavior for Current.")]
        public static IContextFactory Current
        {
            get
            {
                if (currentContextFactory == null)
                {
                    lock (SyncRoot)
                    {
                        if (currentContextFactory == null)
                        {
                            Log.Error(Resources.NoCurrentContextFactory);
                            throw new BackstageException(Resources.NoCurrentContextFactory);
                        }
                    }
                }

                return currentContextFactory;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a current context factory has been defined.
        /// </summary>
        public static bool HasCurrent
        {
            get
            {
                return currentContextFactory != null;
            }
        }

        /// <summary>
        /// Starts a new <see cref="IContextFactory"/> and sets it as current.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <returns>
        /// The <see cref="IContextFactory"/>.
        /// </returns>
        public static IContextFactory StartNew(ContextFactoryConfiguration configuration)
        {
            configuration.ThrowIfNull("configuration");
            lock (SyncRoot)
            {
                if (currentContextFactory != null)
                {
                    var oldContextFactory = currentContextFactory;
                    currentContextFactory = null;
                    oldContextFactory.Dispose();
                }

                var contextFactory = new DefaultContextFactory(configuration);
                contextFactory.Start();
                currentContextFactory = contextFactory;
                return contextFactory;
            }
        }
    }
}
