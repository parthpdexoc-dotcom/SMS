namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Result of employee type upsert when <see cref="EmployeeTypeDto.SameRankIds"/> sync runs (update with explicit same-rank list).
    /// Use with query <c>includeAffectedSameRankTypes=true</c> on POST/PUT.
    /// </summary>
    public class EmployeeTypeUpsertResultDto
    {
        /// <summary>The saved parent employee type (includes <see cref="EmployeeTypeDto.SameRankIds"/>).</summary>
        public EmployeeTypeDto EmployeeType { get; set; } = null!;

        /// <summary>
        /// Types whose <see cref="EmployeeTypeDto.Rank"/> was set to the parent's rank. Null when not requested or not applicable (e.g. create, or SameRankIds omitted).
        /// </summary>
        public IReadOnlyList<EmployeeTypeDto>? AffectedSameRankTypes { get; set; }
    }
}
