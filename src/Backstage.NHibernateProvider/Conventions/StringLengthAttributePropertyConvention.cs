namespace Backstage.NHibernateProvider.Conventions
{
    using System.ComponentModel.DataAnnotations;

    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.Instances;

    /// <summary>
    /// Property convention for <see cref="StringLengthAttribute"/>.
    /// </summary>
    public class StringLengthAttributePropertyConvention : AttributePropertyConvention<StringLengthAttribute>
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
        protected override void Apply(StringLengthAttribute attribute, IPropertyInstance instance)
        {
            instance.Length(attribute.MaximumLength);
        }
    }
}
