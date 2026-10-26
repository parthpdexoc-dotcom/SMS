using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service implementation for HolidayDay operations.
    /// </summary>
    public class HolidayDayService : GenericService<HolidayDayDto, HolidayDay>, IHolidayDayService
    {
        private readonly IMapper _mapper;
        private readonly IHolidayDayRepository _repository;
        private readonly IDepartmentService _departmentService;
        private readonly ITokenParser? _tokenParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="HolidayDayService"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="departmentService">The department service.</param>
        /// <param name="tokenParser">The token parser.</param>
        public HolidayDayService(
            IHolidayDayRepository repository,
            IMapper mapper,
            IDepartmentService departmentService,
            ITokenParser? tokenParser = null)
            : base(repository, mapper, "HolidayDayID", tokenParser)
        {
            _mapper = mapper;
            _repository = repository;
            _departmentService = departmentService;
            _tokenParser = tokenParser;
        }

        /// <summary>
        /// Upserts a holiday day record.
        /// </summary>
        /// <param name="dto">The DTO to upsert.</param>
        /// <returns>The upserted DTO.</returns>
        public override async Task<HolidayDayDto> UpsertAsync(HolidayDayDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // Validate for duplicate Name within the same department
            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                await ValidateDuplicateAsync(
                    propertyName: "Name",
                    value: dto.Name,
                    dto: dto,
                    isDepartmentScoped: true,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_HOLIDAY_NAME);
            }
            return await base.UpsertAsync(dto);
        }

        public override Task<PagedList<HolidayDayDto>> SearchAsync(
            string? searchTerm,
            PageParam? pageParam = null,
            Expression<Func<HolidayDay, bool>>? filter = null,
            Expression<Func<HolidayDay, object>>[]? includes = null,
            params string[] searchableProperties)
        {
            Expression<Func<HolidayDay, bool>> baseFilter = x => true;

            if (filter != null)
            {
                var parameter = Expression.Parameter(typeof(HolidayDay), "x");

                var combined = Expression.AndAlso(
                    Expression.Invoke(baseFilter, parameter),
                    Expression.Invoke(filter, parameter));

                baseFilter = Expression.Lambda<Func<HolidayDay, bool>>(combined, parameter);
            }

            var effectiveSearchableProperties =
                (searchableProperties?.Length > 0)
                    ? searchableProperties
                    : new[]
                    {
                nameof(HolidayDay.Name)
                    };

            return base.SearchAsync(
                searchTerm,
                pageParam,
                baseFilter,
                includes,
                effectiveSearchableProperties
            );
        }
    }
}
