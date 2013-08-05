namespace Backstage
{
    using Common.Logging;

    /// <summary>
    /// Convenience holder for the current <see cref="IContext"/>.
    /// </summary>
    public static class Context
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the current <see cref="IContext"/>.
        /// Uses the current <see cref="ICurrentContextHolder"/>.
        /// </summary>
        /// <exception cref="BackstageException">
        /// If no current context defined.
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Expected behavior for Current.")]
        public static IContext Current
        {
            get
            {
                var currentContext = ContextFactory.Current.CurrentContextHolder.GetCurrentContext();
                if (currentContext == null)
                {
                    Log.Warn(Resources.NoCurrentContext.Format(ContextFactory.Current.CurrentContextHolder));
                    throw new BackstageException(Resources.NoCurrentContext.Format(ContextFactory.Current.CurrentContextHolder));
                }

                return currentContext;
            }

            set
            {
                var currentContext = ContextFactory.Current.CurrentContextHolder.GetCurrentContext();
                if ((currentContext != null) && currentContext.IsReady)
                {
                    throw new BackstageException(Resources.UnableToSetCurrentContextBecauseThereIsAlreadyOne);
                }

                ContextFactory.Current.CurrentContextHolder.SetCurrentContext(value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether a current context has been defined.
        /// </summary>
        public static bool HasCurrent
        {
            get
            {
                return ContextFactory.Current.CurrentContextHolder.GetCurrentContext() != null;
            }
        }
    }
}
