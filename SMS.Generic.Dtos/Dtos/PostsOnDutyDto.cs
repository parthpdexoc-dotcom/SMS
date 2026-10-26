using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for PostsOnDuty operations.
    /// </summary>
    public class PostsOnDutyDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the posts on duty.
        /// </summary>
        public int PostsOnDutyID { get; set; }

        /// <summary>
        /// Gets or sets the division ID.
        /// </summary>
        public int? DivisionID { get; set; }

        /// <summary>
        /// Gets or sets the admin on duty ID.
        /// </summary>]
        public string? AdminOnDutyID { get; set; }

        /// <summary>
        /// Gets or sets the unit name.
        /// </summary>
        public string? UnitName { get; set; }

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        public int? Offset { get; set; }

        /// <summary>
        /// Gets or sets the post name.
        /// </summary>
        public string? PostName { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public int? StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        public int? EndTime { get; set; }

        /// <summary>
        /// Gets or sets the Monday value.
        /// </summary>
        public int? Monday { get; set; }

        /// <summary>
        /// Gets or sets the Tuesday value.
        /// </summary>
        public int? Tueday { get; set; }

        /// <summary>
        /// Gets or sets the Wednesday value.
        /// </summary>
        public int? Wedday { get; set; }

        /// <summary>
        /// Gets or sets the Thursday value.
        /// </summary>
        public int? Thuday { get; set; }

        /// <summary>
        /// Gets or sets the Friday value.
        /// </summary>
        public int? Friday { get; set; }

        /// <summary>
        /// Gets or sets the Saturday value.
        /// </summary>
        public int? Satday { get; set; }

        /// <summary>
        /// Gets or sets the Sunday value.
        /// </summary>
        public int? Sunday { get; set; }

        /// <summary>
        /// Gets or sets the rotation value.
        /// </summary>
        public int? Rotation { get; set; }

        /// <summary>
        /// Gets or sets the four three value.
        /// </summary>
        public int? Fourthree { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public string? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the place holder value.
        /// </summary>
        public int? PlaceHolder { get; set; }

        /// <summary>
        /// Gets or sets the list order.
        /// </summary>
        public string? ListOrder { get; set; }

        /// <summary>
        /// Gets or sets the original start date.
        /// </summary>
        public DateTime? OriginalStartDate { get; set; }
    }
}

