namespace Backstage.NHibernateProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using NHibernate;

    /// <summary>
    /// Base class for <see cref="IPreparedQuery{T}"/>
    /// </summary>
    /// <typeparam name="T">
    /// The type of result.
    /// </typeparam>
    public abstract class BasePreparedQuery<T> : IPreparedQuery<T>
    {
        /// <summary>
        /// The NHibernate session.
        /// </summary>
        private readonly ISession session;

        /// <summary>
        /// The fetch list.
        /// </summary>
        private readonly IList<Expression<Func<T, object>>> fetchList = new List<Expression<Func<T, object>>>();

        /// <summary>
        /// The order by list.
        /// </summary>
        private readonly IList<OrderByPath> orderByList = new List<OrderByPath>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BasePreparedQuery{T}"/> class.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        protected BasePreparedQuery(ISession session)
        {
            session.ThrowIfNull("session");
            this.session = session;
        }

        /// <summary>
        /// Gets the session.
        /// </summary>
        protected ISession Session
        {
            get
            {
                return this.session;
            }
        }

        /// <summary>
        /// Gets the fetch list.
        /// </summary>
        protected IList<Expression<Func<T, object>>> FetchList
        {
            get
            {
                return this.fetchList;
            }
        }

        /// <summary>
        /// Gets the order by list.
        /// </summary>
        protected IList<OrderByPath> OrderByList
        {
            get
            {
                return this.orderByList;
            }
        }

        /// <summary>
        /// Fetches the associated entities.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="PreparedQueryQueryOver{T}"/>.
        /// </returns>
        public virtual IPreparedQuery<T> Fetch(Expression<Func<T, object>> path)
        {
            path.ThrowIfNull("path");
            this.fetchList.Add(path);
            return this;
        }

        /// <summary>
        /// Adds an order by.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="orderType">
        /// The order type.
        /// </param>
        /// <returns>
        /// The <see cref="PreparedQueryQueryOver{T}"/>.
        /// </returns>
        public virtual IPreparedQuery<T> OrderBy(Expression<Func<T, object>> path, OrderType orderType = OrderType.Asc)
        {
            path.ThrowIfNull("path");
            this.orderByList.Add(new OrderByPath { Path = path, OrderType = orderType });
            return this;
        }

        /// <summary>
        /// Adds an order by.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="orderType">
        /// The order type.
        /// </param>
        /// <returns>
        /// The <see cref="PreparedQueryQueryOver{T}"/>.
        /// </returns>
        public virtual IPreparedQuery<T> OrderBy(string path, OrderType orderType = OrderType.Asc)
        {
            path.ThrowIfNullOrEmpty("path");
            this.orderByList.Add(new OrderByPath { PropertyName = path, OrderType = orderType });
            return this;
        }

        /// <summary>
        /// Runs the query and returns paginated results.
        /// </summary>
        /// <param name="page">
        /// The page number, starting at 1.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// The <see cref="IPaginationResult"/>.
        /// </returns>
        public virtual IPaginationResult<T> Paginate(int page = 1, int pageSize = 25)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 0)
            {
                pageSize = 1;
            }

            return this.PaginateImplementation(page, pageSize);
        }

        /// <summary>
        /// Runs the query and returns all the results.
        /// WARNING: you might want to use <see cref="IPreparedQuery{T}.Paginate"/> instead, unless you are sure
        /// that the result set is small.
        /// </summary>
        /// <returns>
        /// The complete results.
        /// </returns>
        public abstract IEnumerable<T> List();

        /// <summary>
        /// Runs the query and returns the number of rows.
        /// </summary>
        /// <returns>
        /// The number of rows.
        /// </returns>
        public abstract int Count();

        /// <summary>
        /// Returns a single instance that matches the query, or null if the query returns no results.
        /// </summary>
        /// <returns>
        /// A single instance that matches the query, or null if the query returns no results.
        /// </returns>
        public abstract T FirstOrDefault();

        /// <summary>
        /// Runs a query with a DISTINCT clause on the target property.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <typeparam name="TProperty">
        /// The type of the property.
        /// </typeparam>
        /// <returns>
        /// The distinct results.
        /// </returns>
        public abstract IEnumerable<TProperty> Distinct<TProperty>(Expression<Func<T, object>> path);

        /// <summary>
        /// Must be implemented to provide pagination results. Arguments will be curated.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// The <see cref="IPaginationResult"/>.
        /// </returns>
        protected abstract IPaginationResult<T> PaginateImplementation(int page, int pageSize);

        /// <summary>
        /// Stores order by.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ByPath", Justification = "Correct words,"),
         System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Internal use.")]
        protected struct OrderByPath
        {
            /// <summary>
            /// Gets or sets the property name.
            /// </summary>
            public string PropertyName { get; set; }

            /// <summary>
            /// Gets or sets the path.
            /// </summary>
            public Expression<Func<T, object>> Path { get; set; }

            /// <summary>
            /// Gets or sets the order type.
            /// </summary>
            public OrderType OrderType { get; set; }
        }
    }
}
