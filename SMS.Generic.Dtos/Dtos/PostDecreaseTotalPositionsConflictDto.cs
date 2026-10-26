namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// 409 conflict payload when decreasing <see cref="PostDto.TotalPostPositions"/>
    /// while the post already has valued call signs that must be removed.
    /// </summary>
    public class PostDecreaseTotalPositionsConflictDto
    {
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// When true, continuing from the UI (with <see cref="PostDto.ConfirmHardDeletePostCallSign"/>)
        /// will hard-delete the selected <c>PostCallSign</c> rows for this post.
        /// </summary>
        public bool RequiresHardDeletePostCallSign { get; set; }

        /// <summary>
        /// Call signs under this post for the UI multi-select hard-delete picker.
        /// </summary>
        public List<PostCallSignDto> CallSigns { get; set; } = new();
    }
}
