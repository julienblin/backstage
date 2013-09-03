namespace Backstage.NHibernateProvider
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Configuration parameters for <see cref="NHContextProviderFactory"/>.
    /// </summary>
    public class NHContextProviderFactoryConfiguration : IValidatable
    {
        /// <summary>
        /// The NHibernate properties.
        /// </summary>
        private IDictionary<string, string> nhProperties;

        /// <summary>
        /// The mapping assemblies.
        /// </summary>
        private IEnumerable<Assembly> mappingAssemblies;

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        [Required]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the database type.
        /// </summary>
        public DatabaseType DatabaseType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to auto-update the database schema on start.
        /// NOT RECOMMENDED IN PRODUCTION.
        /// </summary>
        public bool AutoUpdateSchemaOnStart { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to raise domain events.
        /// </summary>
        public bool RaiseDomainEvents { get; set; }

        /// <summary>
        /// Gets or sets NHibernate properties.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Handy for object initializers.")]
        public IDictionary<string, string> NHProperties
        {
            get
            {
                return this.nhProperties ?? new Dictionary<string, string>();
            }

            set
            {
                this.nhProperties = value;
            }
        }

        /// <summary>
        /// Gets or sets the assemblies to scan for mapping classes and conventions.
        /// If no assemblies are provided, the current directory will be scanned.
        /// </summary>
        public IEnumerable<Assembly> MappingAssemblies
        {
            get
            {
                return this.mappingAssemblies ?? Enumerable.Empty<Assembly>();
            }

            set
            {
                this.mappingAssemblies = value;
            }
        }

        /// <summary>
        /// Gets or sets a function that gets the NHibernate configuration after
        /// it has been constructed by the <see cref="NHContextProviderFactory"/>.
        /// </summary>
        public Action<NHibernate.Cfg.Configuration> ExposeNHibernateConfiguration { get; set; }
    }
}
