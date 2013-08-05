namespace Backstage
{
    using System.Collections.Generic;

    /// <summary>
    /// Pagination result.
    /// </summary>
    public interface IPaginationResult
    {
        /// <summary>
        /// Gets the total number of items (does not depend on the current page or page size).
        /// </summary>
        int TotalItems { get; }

        /// <summary>
        /// Gets the page size.
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Gets the page count (current number of pages available, given the page size).
        /// </summary>
        int PageCount { get; }

        /// <summary>
        /// Gets the current page. 1 is the first page.
        /// </summary>
        int Page { get; }

        /// <summary>
        /// Gets a value indicating whether it has a previous page.
        /// </summary>
        bool HasPreviousPage { get; }

        /// <summary>
        /// Gets a value indicating whether it has a next page.
        /// </summary>
        bool HasNextPage { get; }

        /// <summary>
        /// Gets a value indicating whether it is first page.
        /// </summary>
        bool IsFirstPage { get; }

        /// <summary>
        /// Gets a value indicating whether it is the last page.
        /// </summary>
        bool IsLastPage { get; }
    }

    /// <summary>
    /// Pagination result, including the actual enumerable result.
    /// </summary>
    /// <typeparam name="T">
    /// The type of enumerable result.
    /// </typeparam>
    public interface IPaginationResult<out T> : IPaginationResult
    {
        /// <summary>
        /// Gets the result.
        /// </summary>
        IEnumerable<T> Result { get; }
    }
}
