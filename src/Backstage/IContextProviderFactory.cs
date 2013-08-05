namespace Backstage
{
    using System;

    /// <summary>
    /// The ContextProviderFactory interface.
    /// A context provider factory is responsible for creating specific instances of <see cref="IContextProvider"/>.
    /// <see cref="CreateContextProvider"/> will be called when a context is created.
    /// </summary>
    public interface IContextProviderFactory : IValidatable, IDisposable
    {
        /// <summary>
        /// Starts the context provider. Will be called before any action on it.
        /// </summary>
        /// <param name="contextFactory">
        /// The context factory.
        /// </param>
        void Start(IContextFactory contextFactory);

        /// <summary>
        /// Creates a <see cref="IContextProvider"/>.
        /// </summary>
        /// <param name="context">
        /// The associated context.
        /// </param>
        /// <returns>
        /// The <see cref="IContextProvider"/>.
        /// </returns>
        IContextProvider CreateContextProvider(IContext context);
    }
}
