namespace Backstage.NHibernateProvider.Conventions
{
    using System;

    using FluentNHibernate;
    using FluentNHibernate.Conventions;

    /// <summary>
    /// The default foreign key convention.
    /// </summary>
    public class DefaultForeignKeyConvention : ForeignKeyConvention
    {
        /// <summary>
        /// Returns the key name.
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The key name.
        /// </returns>
        protected override string GetKeyName(Member property, Type type)
        {
            if (property == null)
            {
                return type.Name + "Id"; // many-to-many, one-to-many, join
            }

            return property.Name + "Id"; // many-to-one
        }
    }
}
