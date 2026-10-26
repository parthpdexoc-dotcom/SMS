using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SMS.Generic.Common.Implementations.Paginations
{


    /// <summary>
    /// Class Page Parameters
    /// </summary>
    public class PageParam
    {
        public const int MinOffset = 0, MinPage = 1;
        public const int MinLimit = 25, MinPageSize = 25;

        private int? offset;
        private int? limit;
        private int? page;
        private int? pageSize;
        private string sortDirection;
        private string? sortBy;
        private int totalCount;
        private int totalDebit;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageParam"/> class.
        /// </summary>
        public PageParam()
        {
            Offset = MinOffset;
            Limit = MinLimit;
            sortDirection = "asc";
        }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="PageParam"/> class.
        ///// </summary>
        ///// <param name="offset">The offset.</param>
        ///// <param name="limit">The limit.</param>
        ////public PageParam(int offset, int limit)
        ////{
        ////    this.Offset = offset;
        ////    this.Limit = limit;
        ////}

        /// <summary>
        /// Initializes a new instance of the <see cref="PageParam"/> class.
        /// </summary>
        /// <param name="pageParam">The page parameter.</param>
        /// <exception cref="ArgumentNullException">Thrown when pageParam is null.</exception>
        public PageParam(PageParam pageParam)
        {
            if (pageParam == null)
                throw new ArgumentNullException(nameof(pageParam));

            sortBy = pageParam.SortBy;
            SortDirection = pageParam.SortDirection;
            if (pageParam.IsPageProvided)
            {
                page = pageParam.GetPage();
                pageSize = pageParam.GetPageSize();
            }
            else
            {
                offset = pageParam.GetOffset();
                limit = pageParam.GetLimit();
            }
        }

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        /// <value>
        /// The page.
        /// </value>
        [Range(0, int.MaxValue)]
        public int? Offset
        {
            get
            {
                if (IsPageProvided)
                {
                    return (page - 1) * Limit;
                }

                return offset ?? MinOffset;
            }

            set
            {
                offset = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        [Range(1, int.MaxValue)]
        public int? Limit
        {
            get
            {
                if (IsPageProvided)
                {
                    return PageSize;
                }

                return limit ?? MinLimit;
            }

            set
            {
                limit = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is offset provided.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is offset provided; otherwise, <c>false</c>.
        /// </value>
        private bool IsOffsetProvided
        {
            get
            {
                return offset.HasValue;
            }
        }

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        /// <value>
        /// The page.
        /// </value>
        [Range(1, int.MaxValue)]
        public int? Page
        {
            get
            {
                return page ?? MinPage;
            }

            set
            {
                page = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        [Range(1, int.MaxValue)]
        public int? PageSize
        {
            get
            {
                return pageSize ?? MinPageSize;
            }

            set
            {
                pageSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the sort by field name (optional).
        /// </summary>
        /// <value>
        /// The sort by field name. Can be null if no sorting is required.
        /// </value>
        [ValidateNever]
        public string? SortBy
        {
            get
            {
                return sortBy;
            }
            set
            {
                // Treat empty strings as null to make it truly optional
                sortBy = string.IsNullOrWhiteSpace(value) ? null : value;
            }
        }


        //public int TotalCount
        //{
        //    get
        //    {
        //        return this.totalCount;
        //    }
        //    set
        //    {
        //        this.totalCount = value;
        //    }
        //}


        //public int TotalDebit
        //{
        //    get
        //    {
        //        return this.totalDebit;
        //    }
        //    set
        //    {
        //        this.totalDebit = value;
        //    }
        //}

        /// <summary>
        /// Gets or sets the sort direction.
        /// </summary>
        /// <value>
        /// The sort direction.
        /// </value>
        public string SortDirection
        {
            get
            {
                return sortDirection;
            }
            set
            {
                sortDirection = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is page provided.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is page provided; otherwise, <c>false</c>.
        /// </value>
        private bool IsPageProvided
        {
            get
            {
                return page.HasValue && pageSize.HasValue;
            }
        }

        #region Methods

        /// <summary>
        /// Gets the offset.
        /// </summary>
        /// <returns>the offset.</returns>
        public int? GetOffset()
        {
            return offset;
        }

        /// <summary>
        /// Gets the limit.
        /// </summary>
        /// <returns>the limit.</returns>
        public int? GetLimit()
        {
            return limit;
        }

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <returns>the page.</returns>
        public int? GetPage()
        {
            return page;
        }

        /// <summary>
        /// Gets the size of the page.
        /// </summary>
        /// <returns>the page size.</returns>
        public int? GetPageSize()
        {
            return pageSize;
        }



        #endregion
    }
}
