using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing PTAEO_Code operations using the generic service pattern.
    /// </summary>
    public class PtaeoCodeService : GenericService<PtaeoCodeDto, PtaeoCode>, IPtaeoCodeService
    {
        private readonly IPtaeoCodeRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PtaeoCodeService"/> class.
        /// </summary>
        public PtaeoCodeService(
            IPtaeoCodeRepository repository,
            IMapper mapper,
            ITokenParser? tokenParser = null)
            : base(repository, mapper, "PtaeoCodeID", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task DeleteAllAsync(CancellationToken cancellationToken = default)
        {
            await _repository.DeleteAllAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SelectOptionDto>> GetOptionsAsync(
            string? searchTerm = null,
            Guid? selectedId = null)
        {
            var finalFilter = BuildOptionsFilter(
                e => e.Status || (selectedId.HasValue && e.PtaeoCodeID == selectedId.Value),
                searchTerm: null,
                e => e.ProjectNumber!,
                e => e.TaskNumber!,
                e => e.AwardNumber!,
                e => e.FundNumber!,
                e => e.OrganizationNumber!,
                e => e.OrganizationName!);

            var entities = await _repository.GetAllAsync(finalFilter, null, null).ConfigureAwait(false);

            var options = entities
                .Select(e => new SelectOptionDto
                {
                    Id = e.PtaeoCodeID,
                    Label = BuildPtaeoLabel(e.ProjectNumber, e.TaskNumber, e.AwardNumber, e.FundNumber, e.OrganizationNumber)
                })
                .Where(o => !string.IsNullOrEmpty(o.Label));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                options = options.Where(o =>
                    o.Label.Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            return options
                .OrderBy(o => o.Label, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string BuildPtaeoLabel(
            string? projectNumber,
            string? taskNumber,
            string? awardNumber,
            string? fundNumber,
            string? organizationNumber)
        {
            var parts = new[] { projectNumber, taskNumber, awardNumber, fundNumber, organizationNumber }
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p!.Trim())
                .ToArray();
            return parts.Length > 0 ? string.Join(".", parts) : string.Empty;
        }
    }
}
