﻿namespace Backstage
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Transactions;

    using Common.Logging;

    /// <summary>
    /// <see cref="IContextFactory"/> implementation.
    /// </summary>
    internal class DefaultContextFactory : IContextFactory
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
        /// The configuration.
        /// </summary>
        private readonly ContextFactoryConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultContextFactory"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public DefaultContextFactory(ContextFactoryConfiguration configuration)
        {
            this.configuration = configuration;
            this.IsReady = false;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="DefaultContextFactory"/> class. 
        /// </summary>
        ~DefaultContextFactory()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets a value indicating whether the context factory is ready and can start new <see cref="IContext"/>.
        /// </summary>
        public bool IsReady { get; private set; }

        /// <summary>
        /// Gets the <see cref="IContextProviderFactory"/>.
        /// </summary>
        public IContextProviderFactory ContextProviderFactory
        {
            get
            {
                return this.configuration.ContextProviderFactory;
            }
        }

        /// <summary>
        /// Gets the <see cref="ICurrentContextHolder"/>.
        /// </summary>
        public ICurrentContextHolder CurrentContextHolder
        {
            get
            {
                return this.configuration.CurrentContextHolder;
            }
        }

        /// <summary>
        /// Gets the <see cref="ISecurityProvider"/>.
        /// </summary>
        public ISecurityProvider SecurityProvider
        {
            get
            {
                return this.configuration.SecurityProvider;
            }
        }

        /// <summary>
        /// Starts the <see cref="IContextFactory"/>.
        /// </summary>
        public void Start()
        {
            lock (SyncRoot)
            {
                try
                {
                    Log.Info(Resources.Starting.Format(this.configuration.ContextProviderFactory));
                    var stopwatch = Stopwatch.StartNew();
                    
                    this.configuration.ContextProviderFactory.EnforceValidation();
                    this.configuration.ContextProviderFactory.Start(this);
                    ScanDomainEventHandlersAssemblies();

                    stopwatch.Stop();
                    Log.Info(Resources.StartedIn.Format(this.configuration.ContextProviderFactory, stopwatch.ElapsedMilliseconds));
                    this.IsReady = true;
                }
                catch (Exception ex)
                {
                    Log.Fatal(Resources.ErrorWhileStarting.Format(this.configuration.ContextProviderFactory), ex);
                    throw new BackstageException(Resources.ErrorWhileStarting.Format(this.configuration.ContextProviderFactory), ex);
                }
            }
        }

        /// <summary>
        /// Creates and starts a new context with default options (TransactionScopeOption.Required, new TransactionOptions()).
        /// </summary>
        /// <returns>
        /// The <see cref="IContext"/>.
        /// </returns>
        public IContext StartNewContext()
        {
            if (!this.IsReady)
            {
                Log.Error(Resources.ContextFactoryNotReady);
                throw new BackstageException(Resources.ContextFactoryNotReady);
            }

            var context = new DefaultContext(this);
            context.Start();
            return context;
        }

        /// <summary>
        /// Creates and starts a new context.
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
        public IContext StartNewContext(TransactionScopeOption transactionScopeOption, TransactionOptions transactionOptions)
        {
            if (!this.IsReady)
            {
                Log.Error(Resources.ContextFactoryNotReady);
                throw new BackstageException(Resources.ContextFactoryNotReady);
            }

            var context = new DefaultContext(this);
            context.Start(transactionScopeOption, transactionOptions);
            return context;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose appropriate resources.
        /// </summary>
        /// <param name="disposing">
        /// true if managed resources must be disposed, false otherwise.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Log.Info(Resources.Disposing.Format(this.configuration.ContextProviderFactory));
                this.configuration.ContextProviderFactory.Dispose();
            }
        }

        /// <summary>
        /// Registers domain events handlers.
        /// </summary>
        private static void ScanDomainEventHandlersAssemblies()
        {
            DomainEvents.ClearSubscriptions();

            foreach (var domainEventHandler in TypeScanner.FindAndBuildImplementationsOf(typeof(IHandleDomainEvent<>)))
            {
                DomainEvents.Subscribe(domainEventHandler);
            }
        }
    }
}
