namespace SMS.Generic.Common.Implementations.Paginations
{
    using System.Collections.Generic;

    /// <summary>
    /// Class paged list
    /// </summary>
    /// <typeparam name="T">The Base Model Type.</typeparam>
    public class PagedList<T> : PageParam where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}"/> class.
        /// </summary>
        public PagedList()
        {
            Data = new List<T>();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}"/> class.
        /// </summary>
        /// <param name="data">The data collection.</param>
        /// <param name="pageParam">The page parameter.</param>
        /// <param name="totalRecords">The total number of records.</param>
        /// <exception cref="ArgumentNullException">Thrown when pageParam is null.</exception>
        public PagedList(IEnumerable<T> data, PageParam pageParam, long totalRecords = 0) : base(pageParam)
        {
            if (pageParam == null)
                throw new ArgumentNullException(nameof(pageParam));

            Data = data?.ToList() ?? new List<T>();
            TotalRecords = totalRecords > 0 ? totalRecords : Data.Count;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}"/> class.
        /// </summary>
        /// <param name="pageParam">The page parameter.</param>
        /// <exception cref="ArgumentNullException">Thrown when pageParam is null.</exception>
        public PagedList(PageParam pageParam) : base(pageParam)
        {
            if (pageParam == null)
                throw new ArgumentNullException(nameof(pageParam));

            Data = new List<T>();
        }

        /// <summary>
        /// Gets or sets the total records.
        /// </summary>
        /// <value>
        /// The total records.
        /// </value>
        public long TotalRecords { get; set; }

        /// <summary>
        /// Gets or sets the data collection.
        /// </summary>
        /// <value>
        /// The data collection.
        /// </value>
        public IReadOnlyList<T> Data { get; set; }

        /// <summary>
        /// Gets a value indicating whether this paged list contains valid data.
        /// </summary>
        public bool IsValid => Data?.Count > 0;

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string? Error { get; set; }

        /// <summary>
        /// Gets a value indicating whether this paged list has an error.
        /// </summary>
        public bool HasError => !string.IsNullOrEmpty(Error);

        /// <summary>
        /// Gets or sets whether the employee has an outstanding ready-to-close post blocking further assignments.
        /// </summary>
        public bool HasOutstandingReadyToClosePost { get; set; }
    }
}
