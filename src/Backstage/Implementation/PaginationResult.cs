namespace Backstage.Implementation
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Default implementation for <see cref="IPaginationResult"/>.
    /// </summary>
    public class PaginationResult : IPaginationResult
    {
        /// <summary>
        /// Gets or sets the total number of items (does not depend on the current page or page size).
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the current page. 1 is the first page.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets the page count (current number of pages available, given the page size).
        /// </summary>
        public int PageCount
        {
            get
            {
                return (this.TotalItems / this.PageSize) + (this.TotalItems % this.PageSize == 0 ? 0 : 1);
            }
        }

        /// <summary>
        /// Gets a value indicating whether it has a previous page.
        /// </summary>
        public bool HasPreviousPage
        {
            get { return this.Page > 1; }
        }

        /// <summary>
        /// Gets a value indicating whether it has a next page.
        /// </summary>
        public bool HasNextPage
        {
            get { return this.Page < this.PageCount; }
        }

        /// <summary>
        /// Gets a value indicating whether it is first page.
        /// </summary>
        public bool IsFirstPage
        {
            get { return this.Page <= 1; }
        }

        /// <summary>
        /// Gets a value indicating whether it is the last page.
        /// </summary>
        public bool IsLastPage
        {
            get { return this.Page >= this.PageCount; }
        }
    }

    /// <summary>
    /// Default implementation for <see cref="IPaginationResult{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of enumerable result.
    /// </typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Generic variant.")]
    public class PaginationResult<T> : PaginationResult, IPaginationResult<T>
    {
        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        public IEnumerable<T> Result { get; set; }
    }
}
