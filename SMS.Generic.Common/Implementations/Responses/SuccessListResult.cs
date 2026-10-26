namespace SMS.Generic.Common.Implementations.Responses
{
    using SMS.Generic.Common.Implementations.Models;
    using System.Collections.Generic;

    /// <summary>
    /// Class success result for collections
    /// </summary>
    /// <typeparam name="T">The Base Model Type.</typeparam>
    public class SuccessListResult<T> : AbstractResult where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessListResult{T}"/> class.
        /// </summary>
        public SuccessListResult()
        {
            Data = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessListResult{T}"/> class.
        /// </summary>
        /// <param name="data">The data collection.</param>
        public SuccessListResult(IEnumerable<T> data)
        {
            Data = data?.ToList() ?? new List<T>();
        }

        /// <summary>
        /// Gets or sets the data collection.
        /// </summary>
        /// <value>
        /// The data collection.
        /// </value>
        public ICollection<T> Data { get; set; }
    }
}
