using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System.Linq.Expressions;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing RequesterAuthorized operations using the generic service pattern.
    /// </summary>
    public class RequesterAuthorizedService : GenericService<RequesterAuthorizedDto, RequesterAuthorized>, IRequesterAuthorizedService
    {
        private readonly IRequesterService _requesterService;

        public RequesterAuthorizedService(
            IRequesterAuthorizedRepository repository,
            IMapper mapper,
            ILogger<RequesterAuthorizedService> logger,
            ITokenParser tokenParser,
            IRequesterService requesterService)
            : base(repository, mapper, "RequesterAuthorizedID", tokenParser)
        {
            _requesterService = requesterService;
        }

        /// <summary>
        /// Inserts a new requester authorized or updates an existing one.
        /// </summary>
        /// <param name="dto">The requester authorized DTO to insert or update.</param>
        /// <returns>The upserted requester authorized DTO.</returns>
        public override async Task<RequesterAuthorizedDto> UpsertAsync(RequesterAuthorizedDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // Validate RequesterID exists if provided
            await ValidateForeignKeyAsync<IRequesterService, RequesterDto>(_requesterService, dto.RequesterID, "Requester");

            // Validate PrimaryPhone is unique
            if (!string.IsNullOrWhiteSpace(dto.PrimaryPhone))
            {
                await ValidateDuplicateAsync(
                    nameof(RequesterAuthorized.PrimaryPhone),
                    dto.PrimaryPhone,
                    dto,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_REQUESTER_AUTHORIZED_PRIMARY_PHONE);
            }

            // Validate PrimaryEmail is unique
            if (!string.IsNullOrWhiteSpace(dto.PrimaryEmail))
            {
                await ValidateDuplicateAsync(
                    nameof(RequesterAuthorized.PrimaryEmail),
                    dto.PrimaryEmail,
                    dto,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_REQUESTER_AUTHORIZED_PRIMARY_EMAIL);
            }

            return await base.UpsertAsync(dto);
        }

        /// <summary>
        /// Gets paged requester authorized records with optional filtering by status and requester ID.
        /// </summary>
        /// <param name="isActive">Filter by status (true = active, false = inactive, null = no filter)</param>
        /// <param name="requesterId">Filter by requester ID (null = no filter)</param>
        /// <param name="pagination">Pagination parameters</param>
        /// <returns>Paged list of requester authorized records</returns>
        public async Task<PagedList<RequesterAuthorizedDto>> GetAllPagedAsyncWithFilters(
        bool? isActive = null,
        Guid? requesterId = null,
        PageParam? pagination = null)
        {
            Expression<Func<RequesterAuthorized, bool>>? filter = null;

            // isActive filter
            if (isActive.HasValue)
            {
                Expression<Func<RequesterAuthorized, bool>> statusFilter =
                    isActive.Value
                        ? x => x.Status == true
                        : x => x.Status != true;

                filter = filter == null
                    ? statusFilter
                    : ExpressionCombiner.AndAlso(filter, statusFilter);
            }

            // requesterId filter
            if (requesterId.HasValue && requesterId.Value != Guid.Empty)
            {
                Expression<Func<RequesterAuthorized, bool>> byRequester = x => x.RequesterID == requesterId.Value;
                filter = filter == null
                    ? byRequester
                    : ExpressionCombiner.AndAlso(filter, byRequester);
            }

            return await GetAllPagedAsync(filter, pagination, null);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SelectOptionDto>> GetOptionsAsync(
            string? searchTerm = null,
            Guid? requesterId = null,
            Guid? selectedId = null)
        {
            Expression<Func<RequesterAuthorized, bool>> filter = x =>
                (x.Status == true || (selectedId.HasValue && x.RequesterAuthorizedID == selectedId.Value))
                && (!requesterId.HasValue || requesterId.Value == Guid.Empty || x.RequesterID == requesterId.Value);

            // Keep label EF-translatable (no string.Trim) — same pattern as Employee GetOptions.
            return await GetOptionsAsync(
                x => new SelectOptionDto
                {
                    Id = x.RequesterAuthorizedID,
                    Label = (x.LastName ?? string.Empty) + ", " + (x.FirstName ?? string.Empty)
                },
                filter,
                searchTerm,
                x => x.FirstName!,
                x => x.LastName!).ConfigureAwait(false);
        }
    }
}


