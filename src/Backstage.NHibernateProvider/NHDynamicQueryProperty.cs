namespace Backstage.NHibernateProvider
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    using NHibernate;
    using NHibernate.Criterion;
    using NHibernate.SqlCommand;

    /// <summary>
    /// Used by <see cref="NHDynamicQuery{T}"/> when accessing members (via <see cref="DynamicObject.TryGetMember"/>.
    /// </summary>
    public class NHDynamicQueryProperty : DynamicObject
    {
        /// <summary>
        /// The property info.
        /// </summary>
        private readonly PropertyInfo propertyInfo;

        /// <summary>
        /// The criterions.
        /// </summary>
        private readonly List<ICriterion> criterions = new List<ICriterion>();

        /// <summary>
        /// The joins.
        /// </summary>
        private readonly Dictionary<PropertyInfo, NHDynamicQueryProperty> joins = new Dictionary<PropertyInfo, NHDynamicQueryProperty>();

        /// <summary>
        /// Initializes a new instance of the <see cref="NHDynamicQueryProperty"/> class.
        /// </summary>
        /// <param name="propertyInfo">
        /// The property info.
        /// </param>
        public NHDynamicQueryProperty(PropertyInfo propertyInfo)
        {
            this.propertyInfo = propertyInfo;
        }

        /// <summary>
        /// Applies criterions and joins to the <paramref name="criteria"/>.
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <param name="session">
        /// The session.
        /// </param>
        public virtual void Apply(ICriteria criteria, ISession session)
        {
            foreach (var criterion in this.criterions)
            {
                criteria.Add(criterion);
            }

            if (this.joins.Any())
            {
                var joinCriteria = criteria.CreateCriteria(this.propertyInfo.Name, JoinType.InnerJoin);
                foreach (var join in this.joins.Values)
                {
                    join.Apply(joinCriteria, session);
                }
            }
        }

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param><param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result"/>.</param>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var property = this.propertyInfo.PropertyType.GetProperty(binder.Name);
            if (typeof(IEnumerable).IsAssignableFrom(this.propertyInfo.PropertyType)
                && this.propertyInfo.PropertyType.IsGenericType)
            {
                var targetType = this.propertyInfo.PropertyType.GetGenericArguments()[0];
                property = targetType.GetProperty(binder.Name);
            }

            if (property == null)
            {
                throw new BackstageException(string.Format(CultureInfo.InvariantCulture, Resources.UnableToFindPropertyOnType, binder.Name, this.propertyInfo.PropertyType));
            }

            if (!this.joins.ContainsKey(property))
            {
                this.joins[property] = new NHDynamicQueryProperty(property);
            }

            result = this.joins[property];
            return true;
        }

        /// <summary>
        /// Applies an "equal" constraint to the property.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Eq", Justification = "Shorter form better suits style.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Eq", Justification = "Shorter form better suits style.")]
        public void Eq(object value)
        {
            this.criterions.Add(Restrictions.Eq(this.propertyInfo.Name, value));
        }

        /// <summary>
        /// Applies a "like" constraint to the property - with MatchMode Anywhere.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void Like(string value)
        {
            this.Like(value, null);
        }

        /// <summary>
        /// Applies a "like" constraint to the property.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="matchMode">
        /// The match mode.
        /// </param>
        public void Like(string value, MatchMode matchMode)
        {
            if (matchMode == null)
            {
                matchMode = MatchMode.Anywhere;
            }

            this.criterions.Add(Restrictions.Like(this.propertyInfo.Name, value, matchMode));
        }

        /// <summary>
        /// Applies a case-insensitive "like", similar to Postgres "ilike" operator - with MatchMode Anywhere.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void InsensitiveLike(string value)
        {
            this.InsensitiveLike(value, null);
        }

        /// <summary>
        /// Applies a case-insensitive "like", similar to Postgres "ilike" operator.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="matchMode">
        /// The match mode.
        /// </param>
        public void InsensitiveLike(string value, MatchMode matchMode)
        {
            if (matchMode == null)
            {
                matchMode = MatchMode.Anywhere;
            }

            this.criterions.Add(Restrictions.InsensitiveLike(this.propertyInfo.Name, value, matchMode));
        }

        /// <summary>
        /// Applies a "greater than" constraint to the property.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Gt", Justification = "Shorter form better suits style.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gt", Justification = "Shorter form better suits style.")]
        public void Gt(object value)
        {
            this.criterions.Add(Restrictions.Gt(this.propertyInfo.Name, value));
        }

        /// <summary>
        /// Applies a "greater than or equal" constraint to the property.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ge", Justification = "Shorter form better suits style.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ge", Justification = "Shorter form better suits style.")]
        public void Ge(object value)
        {
            this.criterions.Add(Restrictions.Ge(this.propertyInfo.Name, value));
        }

        /// <summary>
        /// Applies a "less than" constraint to the property.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Lt", Justification = "Shorter form better suits style.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Lt", Justification = "Shorter form better suits style.")]
        public void Lt(object value)
        {
            this.criterions.Add(Restrictions.Lt(this.propertyInfo.Name, value));
        }

        /// <summary>
        /// Applies a "less than or equal" constraint to the property.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Le", Justification = "Shorter form better suits style.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Le", Justification = "Shorter form better suits style.")]
        public void Le(object value)
        {
            this.criterions.Add(Restrictions.Le(this.propertyInfo.Name, value));
        }

        /// <summary>
        /// Applies a "between" constraint to the property.
        /// </summary>
        /// <param name="lo">
        /// The lo.
        /// </param>
        /// <param name="hi">
        /// The hi.
        /// </param>
        public void Between(object lo, object hi)
        {
            this.criterions.Add(Restrictions.Between(this.propertyInfo.Name, lo, hi));
        }

        /// <summary>
        /// Applies an "in" constraint to the property.
        /// </summary>
        /// <param name="value">
        /// The first value.
        /// </param>
        /// <param name="values">
        /// The subsequent values.
        /// </param>
        public void In(object value, params object[] values)
        {
            var collection = new ArrayList { value };
            if (values != null)
            {
                collection.AddRange(values);
            }

            this.In((ICollection)values);
        }

        /// <summary>
        /// Applies an "in" constraint to the property.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <typeparam name="TInValue">
        /// The type of in values.
        /// </typeparam>
        public void In<TInValue>(IEnumerable<TInValue> values)
        {
            this.In((ICollection)values);
        }

        /// <summary>
        /// Applies an "in" constraint to the property.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        public void In(ICollection values)
        {
            this.criterions.Add(Restrictions.In(this.propertyInfo.Name, values));
        }

        /// <summary>
        /// Applies an "is null" constraint to the property.
        /// </summary>
        /// <param name="value">
        /// True to apply "is null", false to apply "is not null".
        /// </param>
        public void IsNull(bool value = true)
        {
            this.criterions.Add(
                value ? Restrictions.IsNull(this.propertyInfo.Name) : Restrictions.IsNotNull(this.propertyInfo.Name));
        }

        /// <summary>
        /// Applies an "is not null" constraint to the property.
        /// </summary>
        public void IsNotNull()
        {
            this.criterions.Add(Restrictions.IsNotNull(this.propertyInfo.Name));
        }

        /// <summary>
        /// Applies an "is empty" constraint to the property.
        /// </summary>
        /// <param name="value">
        /// True to apply "is empty", false to apply "is not empty".
        /// </param>
        public void IsEmpty(bool value = true)
        {
            this.criterions.Add(
                value ? Restrictions.IsEmpty(this.propertyInfo.Name) : Restrictions.IsNotEmpty(this.propertyInfo.Name));
        }

        /// <summary>
        /// Applies an "is not empty" constraint to the property.
        /// </summary>
        public void IsNotEmpty()
        {
            this.criterions.Add(Restrictions.IsNotEmpty(this.propertyInfo.Name));
        }
    }
}
