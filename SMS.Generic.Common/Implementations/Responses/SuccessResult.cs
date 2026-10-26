namespace SMS.Generic.Common.Implementations.Responses
{
    using SMS.Generic.Common.Exceptions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    /// <summary>
    /// Class success result for single items
    /// </summary>
    /// <typeparam name="T">Item class</typeparam>
    public class SuccessResult<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessResult{T}"/> class.
        /// </summary>
        public SuccessResult()
        {
            Message = string.Empty;
            Data = null;
        }
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        /// <value>
        /// The item.
        /// </value>
        public T? Data { get; set; }
        
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid
        {
            get
            {
                if (Data is IEnumerable<T> enumerable)
                {
                    return enumerable.Any();
                }

                return Data != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is success status code.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is success status code; otherwise, <c>false</c>.
        /// </value>
        public bool IsSuccessStatusCode
        {
            get
            {
                return Status >= (int)HttpStatusCode.OK && Status < (int)HttpStatusCode.Ambiguous;
            }
        }

        /// <summary>
        /// List of errors
        /// </summary>
        public List<ErrorDetail> Errors { get; set; }
    }
}
