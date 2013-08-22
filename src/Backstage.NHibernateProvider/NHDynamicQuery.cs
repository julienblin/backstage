namespace Backstage.NHibernateProvider
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Globalization;
    using System.Reflection;

    using NHibernate;

    /// <summary>
    /// A dynamic query, that uses <see cref="DynamicObject"/> to create queries based on entities.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity and the result type for <see cref="IPreparedQuery{T}"/>.
    /// </typeparam>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// public class Employee : NHEntity<int>
    /// {
    ///     public virtual string Name { get; set; }
    /// 
    ///     public virtual DateTime StartDate { get; set; }
    /// 
    ///     public virtual Employee Boss { get; set; }
    /// }
    /// 
    /// dynamic query = new NHDynamicQuery<Employee>();
    /// query.Name.Eq("Foo");
    /// query.StartDate.Gt(new DateTime(2010, 01, 01));
    /// query.Boss.Name.Like("Bar");
    /// Context.Current.Fullfill(query).Paginate();
    /// ]]>
    /// </code>
    /// </example>
    public class NHDynamicQuery<T> : DynamicObject, IQuery<IPreparedQuery<T>>, INHQuery
        where T : class, IEntity
    {
        /// <summary>
        /// The property assignations.
        /// </summary>
        private readonly Dictionary<PropertyInfo, NHDynamicQueryProperty> propertyAssignations = new Dictionary<PropertyInfo, NHDynamicQueryProperty>();

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param><param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result"/>.</param>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var property = typeof(T).GetProperty(binder.Name);
            if (property == null)
            {
                throw new BackstageException(string.Format(CultureInfo.InvariantCulture, Resources.UnableToFindPropertyOnType, binder.Name, typeof(T)));
            }

            if (!this.propertyAssignations.ContainsKey(property))
            {
                this.propertyAssignations[property] = new NHDynamicQueryProperty(property);
            }

            result = this.propertyAssignations[property];
            return true;
        }

        /// <summary>
        /// Fulfills the query - called by the NHibernate <see cref="IContextProvider"/>.
        /// </summary>
        /// <param name="contextProvider">
        /// The context provider.
        /// </param>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <returns>
        /// The result.
        /// </returns>
        public object Fulfill(IContextProvider contextProvider, ISession session)
        {
            return new PreparedQueryCriteria<T>(session, this.BuildCriteria(session));
        }

        /// <summary>
        /// Builds the <see cref="ICriteria"/>.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <returns>
        /// The <see cref="ICriteria"/>.
        /// </returns>
        protected virtual ICriteria BuildCriteria(ISession session)
        {
            var query = session.CreateCriteria<T>();

            foreach (var propertyAssignation in this.propertyAssignations.Values)
            {
                propertyAssignation.Apply(query, session);
            }

            return query;
        }
    }
}
