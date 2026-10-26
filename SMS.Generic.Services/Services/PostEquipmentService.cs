using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Helpers;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing post equipment operations using the generic service pattern.
    /// </summary>
    public class PostEquipmentService : GenericService<PostEquipmentDto, PostEquipment>, IPostEquipmentService
    {
        private readonly IPostEquipmentRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly ITokenParser _tokenParser;
        private readonly IPostService _postService;
        private readonly IDepartmentEquipmentService _departmentEquipmentService;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostEquipmentService"/> class.
        /// </summary>
        public PostEquipmentService(
            IPostEquipmentRepository repository,
            IMapper mapper,
            ILogger<PostEquipmentService> logger,
            ITokenParser tokenParser,
            IPostService postService,
            IDepartmentEquipmentService departmentEquipmentService,
            IServiceProvider serviceProvider) : base(repository, mapper, "PostEquipmentID", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _tokenParser = tokenParser;
            _postService = postService;
            _departmentEquipmentService = departmentEquipmentService;
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public override async Task<PostEquipmentDto> UpsertAsync(PostEquipmentDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            await ValidateForeignKeyAsync<IPostService, PostDto>(_postService, dto.PostID, "Post");
            await ValidateForeignKeyAsync<IDepartmentEquipmentService, DepartmentEquipmentDto>(_departmentEquipmentService, dto.DepartmentEquipmentID, "DepartmentEquipment");

            await ValidateCompositeKeyDuplicateAsync(
                dto: dto,
                filterExpression: e => e.PostID == dto.PostID && e.DepartmentEquipmentID == dto.DepartmentEquipmentID,
                errorMessage: ModelValidationErrorCodes.DUPLICATE_JOB_EQUIPMENT_ASSIGNMENT);

            var requestedQuantity = dto.EquipmentQuantity;
            dto.EquipmentQuantity = EquipmentAssignmentRules.NormalizeStoredQuantity(
                dto.IsVehicle,
                EquipmentAssignmentRules.IsPostPerJob(dto),
                dto.EquipmentQuantity);

            if (dto.IsVehicle && !EquipmentAssignmentRules.IsPostPerJob(dto) && requestedQuantity > 1)
            {
                _logger.LogInformation(
                    "Capping Per Job Position vehicle quantity from {RequestedQuantity} to 1 for PostID {PostId}.",
                    requestedQuantity,
                    dto.PostID);
            }

            // Before updating, freeze allocations for all shifts of this post that have already started or entered the 2-hour freeze window
            if (dto.PostID.HasValue && dto.PostID.Value != Guid.Empty)
            {
                try
                {
                    var postDto = await _postService.GetByIdAsync(dto.PostID.Value).ConfigureAwait(false);
                    if (postDto?.RequestID.HasValue == true && postDto.RequestID.Value != Guid.Empty)
                    {
                        var reqSvc = _serviceProvider.GetRequiredService<IRequestService>();
                        var req = await reqSvc.GetByIdAsync(postDto.RequestID.Value).ConfigureAwait(false);
                        if (req != null && req.DepartmentID != Guid.Empty)
                        {
                            var departmentNow = GetCurrentDepartmentDateTime();
                            var asOfTime = departmentNow.AddHours(2);
                            var empSvc = _serviceProvider.GetRequiredService<IEmployeeService>();
                            var master = await empSvc.GetMasterEmployeeByDepartmentIdAsync(req.DepartmentID).ConfigureAwait(false);
                            if (master?.EmployeeID != null && master.EmployeeID != Guid.Empty)
                            {
                                var masterDisplayName = PersonNameFormatHelper.FormatLastNameCommaFirstName(master.LastName, master.FirstName) ?? "Master System";
                                var vehicleSvc = _serviceProvider.GetRequiredService<IPostPositionVehicleService>();
                                await vehicleSvc.FinalizeStartedAllocationsForDepartmentAsync(
                                    req.DepartmentID,
                                    asOfTime,
                                    master.EmployeeID.Value,
                                    masterDisplayName).ConfigureAwait(false);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to finalize pre-shift allocations before updating PostEquipment for PostID {PostId}.", dto.PostID);
                }
            }

            PostEquipmentDto? before = null;
            if (dto.PostEquipmentID is Guid existingId && existingId != Guid.Empty)
                before = await GetByIdAsync(existingId).ConfigureAwait(false);

            var shouldReshuffle = before == null || HasVehicleEquipmentAssignmentChanged(before, dto);
            var saved = await base.UpsertAsync(dto, isSuperUser);

            if (shouldReshuffle)
            {
                var postId = saved.PostID ?? dto.PostID;
                if (postId is Guid id && id != Guid.Empty)
                    await _postService.NotifyFutureAssigneesOfVehicleEquipmentChangeAsync(id).ConfigureAwait(false);
            }

            return saved;
        }


        /// <inheritdoc />
        public override async Task<bool> DeleteAsync<TKey>(TKey id)
        {
            if (id is not Guid equipmentId || equipmentId == Guid.Empty)
                return await base.DeleteAsync(id).ConfigureAwait(false);

            var existing = await GetByIdAsync(equipmentId).ConfigureAwait(false);
            if (existing?.PostID is not Guid postId || postId == Guid.Empty)
                return await base.DeleteAsync(id).ConfigureAwait(false);

            var deleted = existing.IsVehicle
                ? await _postService.ExecuteVehicleEquipmentChangeAsync(postId, true, () => base.DeleteAsync(id)).ConfigureAwait(false)
                : await base.DeleteAsync(id).ConfigureAwait(false);

            if (deleted)
                await _postService.NotifyFutureAssigneesOfVehicleEquipmentChangeAsync(postId).ConfigureAwait(false);

            return deleted;
        }

        /// <inheritdoc />
        //public override async Task<bool> DeleteAsync<TKey>(TKey id)
        //{
        //    Guid? postId = null;
        //    if (id is Guid guidId && guidId != Guid.Empty)
        //    {
        //        var existing = await GetByIdAsync(guidId).ConfigureAwait(false);
        //        postId = existing?.PostID;
        //    }

        //    var deleted = await base.DeleteAsync(id).ConfigureAwait(false);
        //    if (deleted && postId is Guid pid && pid != Guid.Empty)
        //        await _postService.NotifyFutureAssigneesOfVehicleEquipmentChangeAsync(pid).ConfigureAwait(false);
        //    return deleted;
        //}

        /// <inheritdoc />
        public async Task<IReadOnlyList<PostEquipmentWithNameDto>> GetByPostIdWithNamesAsync(Guid postId, PageParam? pagination = null)
        {
            var items = (await GetAllAsync(pe => pe.PostID == postId, null, pagination).ConfigureAwait(false)).ToList();
            if (items.Count == 0)
                return Array.Empty<PostEquipmentWithNameDto>();

            var deptIds = items
                .Select(x => x.DepartmentEquipmentID.GetValueOrDefault())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            var nameByDeptId = deptIds.Count > 0
                ? await _departmentEquipmentService.GetEquipmentDisplayNamesByIdsAsync(deptIds).ConfigureAwait(false)
                : new Dictionary<Guid, string?>();

            var result = new List<PostEquipmentWithNameDto>(items.Count);
            foreach (var dto in items)
            {
                var row = _mapper.Map<PostEquipmentWithNameDto>(dto);
                var deptId = dto.DepartmentEquipmentID.GetValueOrDefault();
                if (deptId != Guid.Empty && nameByDeptId.TryGetValue(deptId, out var name))
                    row.EquipmentName = name;
                result.Add(row);
            }

            return result;
        }

        private static bool HasVehicleEquipmentAssignmentChanged(PostEquipmentDto before, PostEquipmentDto after) =>
            before.DepartmentEquipmentID != after.DepartmentEquipmentID
            || before.EquipmentQuantity != after.EquipmentQuantity
            || before.EquipmentPerPostPosition != after.EquipmentPerPostPosition
            || before.IsVehicle != after.IsVehicle
            || before.StartDate != after.StartDate
            || before.EndDate != after.EndDate;
    }
}
