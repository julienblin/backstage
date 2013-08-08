namespace Backstage
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// <see cref="IContextProviderFactory"/> that allows multiple <see cref="IContextProviderFactory"/> to be combined.
    /// </summary>
    public class CompositeContextProviderFactory : IContextProviderFactory
    {
        /// <summary>
        /// The selection function.
        /// </summary>
        private readonly Func<IEnumerable<IContextProvider>, Type, IContextProvider> selectionFunction;

        /// <summary>
        /// The factories.
        /// </summary>
        private readonly IContextProviderFactory[] factories;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeContextProviderFactory"/> class.
        /// </summary>
        /// <param name="selectionFunction">
        /// The selection function.
        /// </param>
        /// <param name="factories">
        /// The factories to combine.
        /// </param>
        public CompositeContextProviderFactory(Func<IEnumerable<IContextProvider>, Type, IContextProvider> selectionFunction, params IContextProviderFactory[] factories)
        {
            selectionFunction.ThrowIfNull("selectionFunction");
            factories.ThrowIfNull("factories");
            this.selectionFunction = selectionFunction;
            this.factories = factories;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="CompositeContextProviderFactory"/> class. 
        /// </summary>
        ~CompositeContextProviderFactory()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the factories.
        /// </summary>
        public IEnumerable<IContextProviderFactory> Factories
        {
            get
            {
                return this.factories;
            }
        }

        /// <summary>
        /// Gets the selection function.
        /// </summary>
        internal Func<IEnumerable<IContextProvider>, Type, IContextProvider> SelectionFunction
        {
            get
            {
                return this.selectionFunction;
            }
        }

        /// <summary>
        /// Starts the context provider. Will be called before any action on it.
        /// </summary>
        /// <param name="contextFactory">
        /// The context factory.
        /// </param>
        public void Start(IContextFactory contextFactory)
        {
            foreach (var factory in this.Factories)
            {
                factory.Start(contextFactory);
            }
        }

        /// <summary>
        /// Creates a <see cref="IContextProvider"/>.
        /// </summary>
        /// <param name="context">
        /// The associated context.
        /// </param>
        /// <returns>
        /// The <see cref="IContextProvider"/>.
        /// </returns>
        public IContextProvider CreateContextProvider(IContext context)
        {
            return new CompositeContextProvider(this, context);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
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
                foreach (var factory in this.Factories)
                {
                    factory.Dispose();
                }
            }
        }
    }
}
