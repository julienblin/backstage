namespace Backstage.NHibernateProvider.Conventions
{
    using System.ComponentModel.DataAnnotations;

    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.Instances;

    /// <summary>
    /// Property convention for <see cref="RequiredAttribute"/>.
    /// </summary>
    public class RequiredAttributePropertyConvention : AttributePropertyConvention<RequiredAttribute>
    {
        /// <summary>
        /// Apply the convention.
        /// </summary>
        /// <param name="attribute">
        /// The attribute.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        protected override void Apply(RequiredAttribute attribute, IPropertyInstance instance)
        {
            instance.Not.Nullable();
        }
    }
}
