using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for Post operations.
    /// </summary>
    public class RosterDto : BaseDto
    {
        public class RosterResponseDto
        {
            public string? RequesterName { get; set; } = string.Empty;
            public string RosterDate { get; set; } = string.Empty;
            public List<RosterTableDto> Tables { get; set; } = [];
        }
        public class RosterTableDto
        {
            public string PostTitle { get; set; } = string.Empty;
            public string? PostDescription { get; set; }
            public string? PayCodeValue { get; set; }
            public List<RosterInformationDto> Rows { get; set; } = [];

            public List<RosterTableDto> CreateRosterTablesFromRows()
            {
                if (!Rows.Any())
                    return [];

                var sections = new List<RosterTableDto>();
                var supervisorRows = GetSupervisorRows();
                var nonSupervisorRows = GetNonSupervisorRows();

                AddSupervisorSection(sections);

                if (!nonSupervisorRows.Any())
                    return sections;

                var levelGroups = GroupRosterRowsByHierarchyLevel();

                var allLevels = GetHierarchyLevelsInDisplayOrder();

                var supervisorLevels = GetSupervisorLevels();

                var sectionRoots = GetRootLevelsForRosterSections();

                foreach (var rootLevel in sectionRoots)
                {
                    var sectionRows = GetRowsForSection(rootLevel, levelGroups, supervisorRows, allLevels);

                    if (!sectionRows.Any())
                        continue;

                    sections.Add(CreateRosterTable(sectionRows));
                }

                return OrderSections(sections);
            }
            public List<RosterInformationDto> GetSupervisorRows()
            {
                return Rows.Where(x => x.IsSupervisorPost).OrderBy(x => x.Level, new LevelComparer()).ToList();
            }
            public List<RosterInformationDto> GetNonSupervisorRows()
            {
                return Rows.Where(x => !x.IsSupervisorPost).ToList();
            }

            private static List<RosterInformationDto> GetRosterRowsForLevel(string level, Dictionary<string, List<RosterInformationDto>> levelGroups)
            {
                return levelGroups.TryGetValue(level, out var rows) ? rows : [];
            }
            private static List<RosterTableDto> OrderSections(List<RosterTableDto> sections)
            {
                return sections.OrderBy(x => x.Rows.FirstOrDefault()?.Level, new LevelComparer()).ToList();
            }
            private static string? FormatPayCodeDisplay(string? payCodeValue)
            {
                if (string.IsNullOrWhiteSpace(payCodeValue))
                    return null;

                return $"{ExcelConstants.PayCode}{payCodeValue.Trim()}";
            }

            private static RosterTableDto CreateRosterTable(List<RosterInformationDto> rows)
            {
                var firstRow = rows.First();
                var firstNonSupervisor = rows.FirstOrDefault(x => !x.IsSupervisorPost);

                if (firstRow.IsSupervisorPost && firstNonSupervisor != null)
                {
                    return new RosterTableDto
                    {
                        PostTitle = firstNonSupervisor.PostTitle ?? string.Empty,
                        PostDescription = firstNonSupervisor.PostDescription,
                        PayCodeValue = FormatPayCodeDisplay(firstNonSupervisor.PayCodeValue),
                        Rows = rows
                    };
                }
                return new RosterTableDto
                {
                    PostTitle = firstRow.PostTitle ?? string.Empty,
                    PostDescription = firstRow.PostDescription,
                    PayCodeValue = FormatPayCodeDisplay(firstRow.PayCodeValue),
                    Rows = rows
                };
            }
            private static List<RosterInformationDto> GetRowsForSection(string rootLevel, Dictionary<string, List<RosterInformationDto>> levelGroups, List<RosterInformationDto> supervisorRows, List<string> allLevels)
            {
                var rows = new List<RosterInformationDto>();

                var rootRows = GetRosterRowsForLevel(rootLevel, levelGroups);

                var supervisorRootRows = supervisorRows.Where(x => x.Level == rootLevel).ToList();

                if (supervisorRootRows.Any())
                    rows.AddRange(supervisorRootRows);
                else
                    rows.AddRange(rootRows);

                var childLevels = GetDescendantLevels(rootLevel, allLevels);

                foreach (var childLevel in childLevels)
                    rows.AddRange(GetRosterRowsForLevel(childLevel, levelGroups));

                return rows.OrderBy(x => x.Level, new LevelComparer()).ToList();
            }
            private static List<string> GetDescendantLevels(string rootLevel, List<string> allLevels)
            {
                bool isTopLevel = GetParentLevel(rootLevel) == null;

                return allLevels
                    .Where(level =>
                    {
                        if (level == rootLevel)
                            return false;

                        if (!IsDescendantLevel(level, rootLevel))
                            return false;

                        if (isTopLevel)
                            return IsDirectChildLevel(level, rootLevel);

                        return true;
                    })
                    .OrderBy(x => x, new LevelComparer()).ToList();
            }
            private static bool IsDirectChildLevel(string childLevel, string parentLevel)
            {
                if (!IsDescendantLevel(childLevel, parentLevel))
                    return false;

                var parentDepth = parentLevel.Split('-').Length;

                var childDepth = childLevel.Split('-').Length;

                return childDepth == parentDepth + 1;
            }
            private static bool IsDescendantLevel(string childLevel, string parentLevel)
            {
                return childLevel.StartsWith(parentLevel + "-");
            }
            private static string? GetParentLevel(string level)
            {
                var parts = level.Split('-');

                if (parts.Length <= 1)
                    return null;

                return string.Join("-",parts.Take(parts.Length - 1));
            }
            private List<string> GetRootLevelsForRosterSections()
            {
                var allLevels = GetHierarchyLevelsInDisplayOrder();

                var supervisorLevels = GetSupervisorLevels();
                var sectionRoots = new HashSet<string>();

                foreach (var supervisorLevel in supervisorLevels)
                {
                    bool isTopLevelSupervisor = GetParentLevel(supervisorLevel) == null;

                    if (isTopLevelSupervisor)
                        continue;

                    bool hasNonSupervisorDescendants = allLevels.Any(level => IsDescendantLevel(level, supervisorLevel));

                    if (hasNonSupervisorDescendants)
                        sectionRoots.Add(supervisorLevel);
                }
                foreach (var level in allLevels)
                {
                    var parentLevel = GetParentLevel(level);

                    if (parentLevel == null)
                    {
                        sectionRoots.Add(level);
                        continue;
                    }

                    bool parentIsSupervisor = supervisorLevels.Contains(parentLevel);

                    bool parentAlreadyRoot = sectionRoots.Contains(parentLevel);

                    if (parentIsSupervisor && !parentAlreadyRoot)
                        sectionRoots.Add(level);

                    else if (DoesLevelHaveChildLevels(level, allLevels) && !parentAlreadyRoot)
                        sectionRoots.Add(level);
                }

                return sectionRoots.OrderBy(x => x, new LevelComparer()).ToList();
            }
            private static bool DoesLevelHaveChildLevels(string level, List<string> allLevels)
            {
                return allLevels.Any(x => x != level && IsDescendantLevel(x, level));
            }
            private void AddSupervisorSection(List<RosterTableDto> sections)
            {
                var supervisorRows = GetSupervisorRows();

                if (!supervisorRows.Any())
                    return;

                var firstSupervisor = supervisorRows.First();

                sections.Add(new RosterTableDto
                {
                    PostTitle = firstSupervisor.PostTitle ?? string.Empty,
                    PostDescription = firstSupervisor.PostDescription,
                    PayCodeValue = FormatPayCodeDisplay(firstSupervisor.PayCodeValue),
                    Rows = supervisorRows
                });
            }
            private HashSet<string> GetSupervisorLevels()
            {
                return GetSupervisorRows()
                        .Where(x => !string.IsNullOrWhiteSpace(x.Level))
                        .Select(x => x.Level!)
                        .ToHashSet();
            }
            private Dictionary<string, List<RosterInformationDto>> GroupRosterRowsByHierarchyLevel()
            {
                return GetNonSupervisorRows().GroupBy(x => x.Level ?? "1").ToDictionary(x => x.Key, x => x.ToList());
            }
            private List<string> GetHierarchyLevelsInDisplayOrder()
            {
                return GroupRosterRowsByHierarchyLevel().Keys.OrderBy(x => x, new LevelComparer()).ToList();
            }
        }
        
        private sealed class LevelComparer : IComparer<string?>
        {
            public int Compare(string? x, string? y)
            {
                x ??= "1";
                y ??= "1";

                var xParts = x.Split('-').Select(int.Parse).ToArray();

                var yParts = y.Split('-').Select(int.Parse).ToArray();

                int max = Math.Max(xParts.Length,yParts.Length);

                for (int i = 0; i < max; i++)
                {
                    int xValue = i < xParts.Length? xParts[i] : 0;

                    int yValue = i < yParts.Length? yParts[i]: 0;

                    if (xValue != yValue)
                        return xValue.CompareTo(yValue);
                }

                return 0;
            }
        }
        public class RosterInformationDto
        {
            public Guid? PostID { get; set; }
            public Guid? RequestID { get; set; }
            public Guid? PostParentID { get; set; }
            public string? Level { get; set; }
            public string? PostTitle { get; set; }
            public string? PostDescription { get; set; }
            public string? PayCodeValue { get; set; }
            public string? PostLocation { get; set; }
            public bool IsSupervisorPost { get; set; }
            public string? CallUnit { get; set; }
            public string? EmployeeName { get; set; }
            public string? EmployeeNumber { get; set; }
            public string? VehicleName { get; set; }
            public Guid? PostPositionID { get; set; }
            public PostPositionStatus? PostPositionStatus { get; set; }
            public RosterInformationDto(Guid? postId, Guid? requestId, Guid? postParentId, string? level, string? postTitle, string? postDescription, string? postLocation, bool isSupervisorPost, string? callUnit, string? employeeName, string? employeeNumber, string? vehicleName, Guid? postPositionID, PostPositionStatus? postPositionStatus,string? paycodeValue)
            {
                PostID = postId;
                RequestID = requestId;
                PostParentID = postParentId;
                Level = level;
                PostTitle = postTitle;
                PostDescription = postDescription;
                PostLocation = postLocation;
                IsSupervisorPost = isSupervisorPost;
                PayCodeValue = paycodeValue;
                CallUnit = callUnit;
                EmployeeName = employeeName;
                EmployeeNumber = employeeNumber;
                VehicleName = vehicleName;
                PostPositionID = postPositionID;
                PostPositionStatus = postPositionStatus;
            }
        }
    }
}
