namespace Backstage.NHibernateProvider
{
    using System;

    using Common.Logging;

    using NHibernate;

    /// <summary>
    /// NHibernate <see cref="IContextProvider"/>. Binds to a <see cref="ISession"/>.
    /// </summary>
    public class NHContextProvider : IContextProvider
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The <see cref="NHContextProviderFactory"/>.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Could become a handy reference.")]
        private readonly NHContextProviderFactory contextProviderFactory;

        /// <summary>
        /// The context.
        /// </summary>
        private readonly IContext context;

        /// <summary>
        /// The session.
        /// </summary>
        private readonly ISession session;

        /// <summary>
        /// Initializes a new instance of the <see cref="NHContextProvider"/> class.
        /// </summary>
        /// <param name="contextProviderFactory">
        /// The <see cref="NHContextProviderFactory"/>.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public NHContextProvider(NHContextProviderFactory contextProviderFactory, IContext context)
        {
            this.contextProviderFactory = contextProviderFactory;
            this.context = context;
            this.session = contextProviderFactory.SessionFactory.OpenSession();
            Log.Debug(Resources.Opened.Format(this.session));
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="NHContextProvider"/> class. 
        /// </summary>
        ~NHContextProvider()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the <see cref="IContext"/>.
        /// </summary>
        public IContext Context
        {
            get
            {
                return this.context;
            }
        }

        /// <summary>
        /// Gets the NHibernate <see cref="ISession"/>.
        /// </summary>
        public ISession Session
        {
            get
            {
                return this.session;
            }
        }

        /// <summary>
        /// Adds an entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Add(IEntity entity)
        {
            entity.ThrowIfNull("entity");
            this.session.Save(entity);
        }

        /// <summary>
        /// Removes an entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Remove(IEntity entity)
        {
            entity.ThrowIfNull("entity");
            this.session.Delete(entity);
        }

        /// <summary>
        /// The reload.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Reload(IEntity entity)
        {
            entity.ThrowIfNull("entity");
            this.session.Refresh(entity);
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
            return this.session.Get<T>(id);
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
            query.ThrowIfNull("query");
            var nhquery = query as INHQuery;

            if (nhquery == null)
            {
                throw new BackstageException(Resources.QueryMustBeNHQuery.Format(query));
            }

            return (T)nhquery.Fulfill(this, this.Session);
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
                if (this.session != null)
                {
                    this.session.Dispose();
                }
            }
        }
    }
}
