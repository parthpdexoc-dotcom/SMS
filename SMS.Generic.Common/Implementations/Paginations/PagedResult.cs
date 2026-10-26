namespace SMS.Generic.Common.Implementations.Paginations
{
    using System.Collections.Generic;
    using SMS.Generic.Common.Implementations.Responses;

    /// <summary>
    /// Class page result
    /// </summary>
    /// <typeparam name="T">The Base Model Type.</typeparam>
    public class PagedResult<T> : SuccessListResult<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PagedResult{T}"/> class.
        /// </summary>
        public PagedResult() : base()
        {
            Pagination = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedResult{T}"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public PagedResult(PagedList<T> result) : base(result.Data)
        {
            Pagination = new Dictionary<string, string>();
            Pagination.Add("totalrecords", result.TotalRecords.ToString());
        }

        /// <summary>
        /// Gets or sets the pagination information.
        /// </summary>
        /// <value>
        /// The pagination information.
        /// </value>
        public IDictionary<string, string> Pagination { get; set; }
    }
}
