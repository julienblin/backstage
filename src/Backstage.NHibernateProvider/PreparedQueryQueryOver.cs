namespace Backstage.NHibernateProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Backstage.Implementation;

    using Common.Logging;

    using NHibernate;
    using NHibernate.Criterion;

    /// <summary>
    /// Implementation of <see cref="IPreparedQuery{T}"/> for the <see cref="IQueryOver{TRoot, TSubtype}"/> API.
    /// </summary>
    /// <typeparam name="T">
    /// The type of result.
    /// </typeparam>
    public class PreparedQueryQueryOver<T> : BasePreparedQuery<T>
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The query over.
        /// </summary>
        private readonly IQueryOver<T, T> queryOver;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreparedQueryQueryOver{T}"/> class.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="queryOver">
        /// The query over.
        /// </param>
        public PreparedQueryQueryOver(ISession session, IQueryOver<T, T> queryOver)
            : base(session)
        {
            queryOver.ThrowIfNull("queryOver");
            this.queryOver = queryOver;
        }

        /// <summary>
        /// Runs the query and returns all the results.
        /// WARNING: you might want to use Paginate method instead, unless you are sure
        /// that the result set is small.
        /// </summary>
        /// <returns>
        /// The complete results.
        /// </returns>
        public override IEnumerable<T> List()
        {
            return this.BuildFinalQueryOver().List<T>();
        }

        /// <summary>
        /// Runs the query and returns the number of rows.
        /// </summary>
        /// <returns>
        /// The number of rows.
        /// </returns>
        public override int Count()
        {
            return this.BuildFinalQueryOver().RowCount();
        }

        /// <summary>
        /// Returns a single instance that matches the query, or null if the query returns no results.
        /// </summary>
        /// <returns>
        /// A single instance that matches the query, or null if the query returns no results.
        /// </returns>
        public override T FirstOrDefault()
        {
            return this.BuildFinalQueryOver().SingleOrDefault<T>();
        }

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
        public override IEnumerable<TProperty> Distinct<TProperty>(Expression<Func<T, object>> path)
        {
            return this.BuildFinalQueryOver().Select(Projections.Distinct(Projections.Property(path))).List<TProperty>();
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
        protected override IPaginationResult<T> PaginateImplementation(int page, int pageSize)
        {
            var query = this.BuildFinalQueryOver();
            var rowCount = query.ToRowCountQuery().FutureValue<int>();

            var items = query.Skip((page - 1) * pageSize).Take(pageSize).Future();
            return new PaginationResult<T>
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = rowCount.Value,
                Result = items
            };
        }

        /// <summary>
        /// Finalizes the <see cref="IQueryOver"/> creation by adding fetches, orders and some.
        /// </summary>
        /// <returns>
        /// The <see cref="IQueryOver"/>.
        /// </returns>
        protected virtual IQueryOver<T, T> BuildFinalQueryOver()
        {
            var result = this.queryOver.Clone();
            result = this.FetchList.Aggregate(result, (current, fetch) => current.Fetch(fetch).Eager);

            foreach (var orderByPath in this.OrderByList)
            {
                if (orderByPath.Path != null)
                {
                    switch (orderByPath.OrderType)
                    {
                        case OrderType.Asc:
                            result.OrderBy(orderByPath.Path).Asc();
                            break;
                        case OrderType.Desc:
                            result.OrderBy(orderByPath.Path).Desc();
                            break;
                        default:
                            Log.Error(Resources.UnrecognizedOrderType.Format(orderByPath.OrderType));
                            throw new BackstageException(Resources.UnrecognizedOrderType.Format(orderByPath.OrderType));
                    }
                }
                else
                {
                    switch (orderByPath.OrderType)
                    {
                        case OrderType.Asc:
                            result.OrderBy(Projections.Property(orderByPath.PropertyName)).Asc();
                            break;
                        case OrderType.Desc:
                            result.OrderBy(Projections.Property(orderByPath.PropertyName)).Desc();
                            break;
                        default:
                            Log.Error(Resources.UnrecognizedOrderType.Format(orderByPath.OrderType));
                            throw new BackstageException(Resources.UnrecognizedOrderType.Format(orderByPath.OrderType));
                    }
                }
            }

            // We flush before executing a query to make sure the vision of the database is accurate.
            this.Session.Flush();

            return result;
        }
    }
}
