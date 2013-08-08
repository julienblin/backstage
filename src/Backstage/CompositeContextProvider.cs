namespace Backstage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Common.Logging;

    /// <summary>
    /// <see cref="IContextProvider"/> implementation that allow the combination of multiple <see cref="IContextProvider"/>.
    /// </summary>
    internal class CompositeContextProvider : IContextProvider
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The composite context provider factory.
        /// </summary>
        private readonly CompositeContextProviderFactory providerFactory;

        /// <summary>
        /// The context providers.
        /// </summary>
        private readonly IList<IContextProvider> contextProviders;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeContextProvider"/> class.
        /// </summary>
        /// <param name="providerFactory">
        /// The composite context provider factory.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public CompositeContextProvider(CompositeContextProviderFactory providerFactory, IContext context)
        {
            providerFactory.ThrowIfNull("compositeContextProviderFactory");
            this.providerFactory = providerFactory;
            this.contextProviders = this.providerFactory.Factories.Select(x => x.CreateContextProvider(context)).ToList();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="CompositeContextProvider"/> class. 
        /// </summary>
        ~CompositeContextProvider()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Adds an entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Add(IEntity entity)
        {
            var selectedContextprovider = this.providerFactory.SelectionFunction(this.contextProviders, entity.GetType());
            if (selectedContextprovider == null)
            {
                Log.Error(Resources.SelectionFunctionReturnedNull.Format(entity.GetType()));
                throw new BackstageException(Resources.SelectionFunctionReturnedNull.Format(entity.GetType()));
            }

            selectedContextprovider.Add(entity);
        }

        /// <summary>
        /// Removes an entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Remove(IEntity entity)
        {
            var selectedContextprovider = this.providerFactory.SelectionFunction(this.contextProviders, entity.GetType());
            if (selectedContextprovider == null)
            {
                Log.Error(Resources.SelectionFunctionReturnedNull.Format(entity.GetType()));
                throw new BackstageException(Resources.SelectionFunctionReturnedNull.Format(entity.GetType()));
            }

            selectedContextprovider.Remove(entity);
        }

        /// <summary>
        /// Reloads the <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Reload(IEntity entity)
        {
            var selectedContextprovider = this.providerFactory.SelectionFunction(this.contextProviders, entity.GetType());
            if (selectedContextprovider == null)
            {
                Log.Error(Resources.SelectionFunctionReturnedNull.Format(entity.GetType()));
                throw new BackstageException(Resources.SelectionFunctionReturnedNull.Format(entity.GetType()));
            }

            selectedContextprovider.Reload(entity);
        }

        /// <summary>
        /// Gets an entity by its id.
        /// </summary>
        /// <param name="id">
        /// The entity id.
        /// </param>
        /// <typeparam name="T">
        /// The type of entity.
        /// </typeparam>
        /// <returns>
        /// The entity.
        /// </returns>
        public T GetById<T>(object id)
        {
            var selectedContextprovider = this.providerFactory.SelectionFunction(this.contextProviders, typeof(T));
            if (selectedContextprovider == null)
            {
                Log.Error(Resources.SelectionFunctionReturnedNull.Format(typeof(T)));
                throw new BackstageException(Resources.SelectionFunctionReturnedNull.Format(typeof(T)));
            }

            return selectedContextprovider.GetById<T>(id);
        }

        /// <summary>
        /// Fulfill a <see cref="IQuery{T}"/>.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <typeparam name="T">
        /// The type or returned result query.
        /// </typeparam>
        /// <returns>
        /// The result.
        /// </returns>
        public T Fulfill<T>(IQuery<T> query)
        {
            var selectedContextprovider = this.providerFactory.SelectionFunction(this.contextProviders, query.GetType());
            if (selectedContextprovider == null)
            {
                Log.Error(Resources.SelectionFunctionReturnedNull.Format(query.GetType()));
                throw new BackstageException(Resources.SelectionFunctionReturnedNull.Format(query.GetType()));
            }

            return selectedContextprovider.Fulfill(query);
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
                foreach (var contextProvider in this.contextProviders)
                {
                    contextProvider.Dispose();
                }
            }
        }
    }
}
