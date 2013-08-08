namespace Backstage
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Configuration object for <see cref="ContextFactory"/>
    /// </summary>
    public class ContextFactoryConfiguration
    {
        /// <summary>
        /// The context provider factory.
        /// </summary>
        private readonly IContextProviderFactory contextProviderFactory;

        /// <summary>
        /// The current context holder.
        /// </summary>
        private readonly ICurrentContextHolder currentContextHolder;

        /// <summary>
        /// The domain event handlers assemblies.
        /// </summary>
        private IEnumerable<Assembly> domainEventHandlersAssemblies;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextFactoryConfiguration"/> class.
        /// </summary>
        /// <param name="contextProviderFactory">
        /// The context provider factory.
        /// </param>
        public ContextFactoryConfiguration(IContextProviderFactory contextProviderFactory)
            : this(contextProviderFactory, new ThreadStaticCurrentContextHolder())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextFactoryConfiguration"/> class.
        /// </summary>
        /// <param name="contextProviderFactory">
        /// The context provider factory.
        /// </param>
        /// <param name="currentContextHolder">
        /// The current Context Holder.
        /// </param>
        public ContextFactoryConfiguration(IContextProviderFactory contextProviderFactory, ICurrentContextHolder currentContextHolder)
        {
            contextProviderFactory.ThrowIfNull("contextProviderFactory");
            currentContextHolder.ThrowIfNull("currentContextHolder");
            this.contextProviderFactory = contextProviderFactory;
            this.currentContextHolder = currentContextHolder;
        }

        /// <summary>
        /// Gets the context provider factory.
        /// </summary>
        public IContextProviderFactory ContextProviderFactory
        {
            get
            {
                return this.contextProviderFactory;
            }
        }

        /// <summary>
        /// Gets the current context holder.
        /// </summary>
        public ICurrentContextHolder CurrentContextHolder
        {
            get
            {
                return this.currentContextHolder;
            }
        }

        /// <summary>
        /// Gets or sets the list of assemblies to scan for <see cref="IHandleDomainEvent{T}"/>.
        /// </summary>
        public IEnumerable<Assembly> DomainEventHandlersAssemblies
        {
            get
            {
                return this.domainEventHandlersAssemblies ?? Enumerable.Empty<Assembly>();
            }

            set
            {
                this.domainEventHandlersAssemblies = value;
            }
        }
    }
}
