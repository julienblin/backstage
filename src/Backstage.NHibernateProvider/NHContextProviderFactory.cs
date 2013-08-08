namespace Backstage.NHibernateProvider
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Backstage.NHibernateProvider.Events;
    using Backstage.NHibernateProvider.Logs;

    using Common.Logging;

    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;

    using NHibernate;
    using NHibernate.Cfg;
    using NHibernate.Event;
    using NHibernate.Tool.hbm2ddl;

    /// <summary>
    /// The NHibernate <see cref="IContextProviderFactory"/>. Binds to a <see cref="ISessionFactory"/>.
    /// </summary>
    public class NHContextProviderFactory : IContextProviderFactory, IValidatableObject
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The configuration.
        /// </summary>
        private readonly NHContextProviderFactoryConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="NHContextProviderFactory"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public NHContextProviderFactory(NHContextProviderFactoryConfiguration configuration)
        {
            configuration.ThrowIfNull("configuration");
            this.configuration = configuration;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="NHContextProviderFactory"/> class. 
        /// </summary>
        ~NHContextProviderFactory()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        [Required]
        public NHContextProviderFactoryConfiguration Configuration
        {
            get
            {
                return this.configuration;
            }
        }

        /// <summary>
        /// Gets the NHibernate configuration.
        /// </summary>
        public Configuration NHConfiguration { get; private set; }

        /// <summary>
        /// Gets the NHibernate <see cref="ISessionFactory"/>.
        /// </summary>
        public ISessionFactory SessionFactory { get; private set; }

        /// <summary>
        /// Starts the context provider. Will be called before any action on it.
        /// </summary>
        /// <param name="contextFactory">
        /// The context factory.
        /// </param>
        public void Start(IContextFactory contextFactory)
        {
            contextFactory.ThrowIfNull("contextFactory");

            this.NHConfiguration = this.BuildNHConfiguration();

            if (this.configuration.AutoUpdateSchemaOnStart)
            {
                Log.Warn(Resources.UpdatingSchema);
                new SchemaUpdate(this.NHConfiguration).Execute(true, true);
            }

            Log.Debug(Resources.BuildingNHibernateSessionFactory);
            this.SessionFactory = this.NHConfiguration.BuildSessionFactory();
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
            return new NHContextProvider(this, context);
        }

        /// <summary>
        /// Determines whether the specified object is valid.
        /// </summary>
        /// <returns>
        /// A collection that holds failed-validation information.
        /// </returns>
        /// <param name="validationContext">The validation context.</param>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return this.configuration.Validate();
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
                if (this.SessionFactory != null)
                {
                    this.SessionFactory.Dispose();
                }
            }
        }

        /// <summary>
        /// Builds the NHibernate configuration object.
        /// </summary>
        /// <returns>
        /// The <see cref="Configuration"/>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "NHibernate configuration necessitates a lot of types.")]
        protected virtual Configuration BuildNHConfiguration()
        {
            Log.Debug(Resources.BuildingNHibernateConfiguration);

            IPersistenceConfigurer fluentDbConfig;

            switch (this.configuration.DatabaseType)
            {
                case DatabaseType.SQLite:
                    fluentDbConfig = string.IsNullOrEmpty(this.configuration.ConnectionString)
                                   ? SQLiteConfiguration.Standard.InMemory()
                                   : SQLiteConfiguration.Standard.ConnectionString(this.configuration.ConnectionString);
                    break;
                case DatabaseType.SqlServer2008:
                    fluentDbConfig = MsSqlConfiguration.MsSql2008;
                    if (!string.IsNullOrEmpty(this.configuration.ConnectionString))
                    {
                        fluentDbConfig = ((MsSqlConfiguration)fluentDbConfig).ConnectionString(this.configuration.ConnectionString);
                    }

                    break;
                default:
                    Log.Fatal(Resources.UnrecognizedDatabaseType.Format(this.configuration.DatabaseType));
                    throw new BackstageException(Resources.UnrecognizedDatabaseType.Format(this.configuration.DatabaseType));
            }

            var fluentConfig = Fluently.Configure().Database(fluentDbConfig);

            foreach (var mappingAssembly in this.configuration.MappingAssemblies)
            {
                fluentConfig.Mappings(m =>
                    {
                        var container = m.FluentMappings.AddFromAssembly(mappingAssembly);
                        container.Conventions.AddAssembly(typeof(NHContextProviderFactory).Assembly);
                        foreach (var conventionAssembly in this.configuration.ConventionAssemblies)
                        {
                            container.Conventions.AddAssembly(conventionAssembly);
                        }
                    });
            }

            var resultConfig = fluentConfig.BuildConfiguration();
            LoggerProvider.SetLoggersFactory(new NHCommonLoggerFactory());

            foreach (var key in this.configuration.NHProperties.Keys)
            {
                resultConfig.SetProperty(key, this.configuration.NHProperties[key]);
            }

            if (this.configuration.RaiseDomainEvents)
            {
                var domainEventListener = new DomainEventsEventListener();
                resultConfig.SetListener(ListenerType.PostInsert, domainEventListener);
                resultConfig.SetListener(ListenerType.PostUpdate, domainEventListener);
                resultConfig.SetListener(ListenerType.PostDelete, domainEventListener);
            }

            return resultConfig;
        }
    }
}
