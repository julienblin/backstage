namespace Backstage.NHibernateProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// A type of query result that allows various final transformations.
    /// </summary>
    /// <typeparam name="T">
    /// The type of result.
    /// </typeparam>
    public interface IPreparedQuery<T>
    {
        /// <summary>
        /// Fetches the associated entities.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="IPreparedQuery{T}"/>.
        /// </returns>
        IPreparedQuery<T> Fetch(Expression<Func<T, object>> path);

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
        /// The <see cref="IPreparedQuery{T}"/>.
        /// </returns>
        IPreparedQuery<T> OrderBy(Expression<Func<T, object>> path, OrderType orderType = OrderType.Asc);

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
        /// The <see cref="IPreparedQuery{T}"/>.
        /// </returns>
        IPreparedQuery<T> OrderBy(string path, OrderType orderType = OrderType.Asc);

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
        IPaginationResult<T> Paginate(int page = 1, int pageSize = 25);

        /// <summary>
        /// Runs the query and returns all the results.
        /// WARNING: you might want to use <see cref="Paginate"/> instead, unless you are sure
        /// that the result set is small.
        /// </summary>
        /// <returns>
        /// The complete results.
        /// </returns>
        IEnumerable<T> List();

        /// <summary>
        /// Runs the query and returns the number of rows.
        /// </summary>
        /// <returns>
        /// The number of rows.
        /// </returns>
        int Count();

        /// <summary>
        /// Returns a single instance that matches the query, or null if the query returns no results.
        /// </summary>
        /// <returns>
        /// A single instance that matches the query, or null if the query returns no results.
        /// </returns>
        T FirstOrDefault();

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
        IEnumerable<TProperty> Distinct<TProperty>(Expression<Func<T, object>> path);
    }
}
