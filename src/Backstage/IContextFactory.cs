namespace Backstage
{
    using System;
    using System.Transactions;

    /// <summary>
    /// The Context Factory is the main entry point for Backstage.
    /// It must be unique per application container, usually configured and started at the beginning.
    /// Can be created using <see cref="ContextFactory"/>.
    /// </summary>
    public interface IContextFactory : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether the context factory is ready and can start new <see cref="IContext"/>.
        /// </summary>
        bool IsReady { get; }

        /// <summary>
        /// Gets the <see cref="IContextProviderFactory"/>.
        /// </summary>
        IContextProviderFactory ContextProviderFactory { get; }

        /// <summary>
        /// Gets the <see cref="ICurrentContextHolder"/>.
        /// </summary>
        ICurrentContextHolder CurrentContextHolder { get; }

        /// <summary>
        /// Starts the <see cref="IContextFactory"/>.
        /// </summary>
        void Start();

        /// <summary>
        /// Creates and starts a new <see cref="IContext"/> with default options (TransactionScopeOption.Required, new TransactionOptions()).
        /// </summary>
        /// <returns>
        /// The <see cref="IContext"/>.
        /// </returns>
        IContext StartNewContext();

        /// <summary>
        /// Creates and starts a new <see cref="IContext"/>.
        /// </summary>
        /// <param name="transactionScopeOption">
        /// The transaction scope option.
        /// </param>
        /// <param name="transactionOptions">
        /// The transaction options.
        /// </param>
        /// <returns>
        /// The <see cref="IContext"/>.
        /// </returns>
        IContext StartNewContext(TransactionScopeOption transactionScopeOption, TransactionOptions transactionOptions);
    }
}
