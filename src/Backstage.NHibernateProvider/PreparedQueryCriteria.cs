namespace Backstage.NHibernateProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Backstage.Implementation;

    using Common.Logging;

    using NHibernate;
    using NHibernate.Criterion;
    using NHibernate.Impl;

    /// <summary>
    /// Implementation of <see cref="IPreparedQuery{T}"/> for the <see cref="ICriteria"/> API.
    /// </summary>
    /// <typeparam name="T">
    /// The type of result.
    /// </typeparam>
    public class PreparedQueryCriteria<T> : BasePreparedQuery<T>
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The criteria.
        /// </summary>
        private readonly ICriteria criteria;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreparedQueryCriteria{T}"/> class.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        public PreparedQueryCriteria(ISession session, ICriteria criteria)
            : base(session)
        {
            criteria.ThrowIfNull("criteria");
            this.criteria = criteria;
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
            return this.BuildFinalCriteria().List<T>();
        }

        /// <summary>
        /// Runs the query and returns the number of rows.
        /// </summary>
        /// <returns>
        /// The number of rows.
        /// </returns>
        public override int Count()
        {
            return this.BuildFinalCriteria().SetProjection(Projections.RowCount()).UniqueResult<int>();
        }

        /// <summary>
        /// Returns a single instance that matches the query, or null if the query returns no results.
        /// </summary>
        /// <returns>
        /// A single instance that matches the query, or null if the query returns no results.
        /// </returns>
        public override T FirstOrDefault()
        {
            return this.BuildFinalCriteria().UniqueResult<T>();
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
            var query = this.BuildFinalCriteria();
            var rowCount = ((ICriteria)query.Clone()).SetProjection(Projections.RowCount()).FutureValue<int>();

            var items =
                query.SetFirstResult((page - 1) * pageSize)
                     .SetMaxResults(pageSize)
                     .Future<T>();
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
        protected virtual ICriteria BuildFinalCriteria()
        {
            var result = (ICriteria)this.criteria.Clone();
            foreach (var fetch in this.FetchList)
            {
                result.SetFetchMode(ExpressionProcessor.FindMemberExpression(fetch.Body), FetchMode.Eager);
            }

            foreach (var orderByPath in this.OrderByList)
            {
                if (orderByPath.Path != null)
                {
                    switch (orderByPath.OrderType)
                    {
                        case OrderType.Asc:
                            result.AddOrder(Order.Asc(ExpressionProcessor.FindMemberExpression(orderByPath.Path)));
                            break;
                        case OrderType.Desc:
                            result.AddOrder(Order.Desc(ExpressionProcessor.FindMemberExpression(orderByPath.Path)));
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
                            result.AddOrder(Order.Asc(orderByPath.PropertyName));
                            break;
                        case OrderType.Desc:
                            result.AddOrder(Order.Desc(orderByPath.PropertyName));
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
