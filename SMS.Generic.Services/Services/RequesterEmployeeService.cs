using AutoMapper;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service implementation for RequesterEmployee operations.
    /// </summary>
    public class RequesterEmployeeService : GenericService<RequesterEmployeeDto, RequesterEmployee>, IRequesterEmployeeService
    {
        private readonly IMapper _mapper;
        private readonly IRequesterEmployeeRepository _repository;
        private readonly IEmployeeService _employeeService;
        private readonly IRequesterManagerService _requesterManagerService;
        private readonly ITokenParser? _tokenParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterEmployeeService"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="employeeService">The employee service.</param>
        /// <param name="requesterManagerService">The requester manager service.</param>
        /// <param name="tokenParser">The token parser.</param>
        public RequesterEmployeeService(
            IRequesterEmployeeRepository repository,
            IMapper mapper,
            IEmployeeService employeeService,
            IRequesterManagerService requesterManagerService,
            ITokenParser? tokenParser = null)
            : base(repository, mapper, "RequesterEmployeeID", tokenParser)
        {
            _mapper = mapper;
            _repository = repository;
            _employeeService = employeeService;
            _requesterManagerService = requesterManagerService;
            _tokenParser = tokenParser;
        }

        /// <summary>
        /// Upserts a requester employee record.
        /// </summary>
        /// <param name="dto">The DTO to upsert.</param>
        /// <returns>The upserted DTO.</returns>
        public override async Task<RequesterEmployeeDto> UpsertAsync(RequesterEmployeeDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // Validate RequesterManagerID foreign key exists
            await ValidateForeignKeyAsync<IRequesterManagerService, RequesterManagerDto>(
                _requesterManagerService,
                dto.RequesterManagerID,
                "RequesterManager");

            // Validate EmployeeID foreign key exists
            await ValidateForeignKeyAsync<IEmployeeService, EmployeeDto>(
                _employeeService,
                dto.EmployeeID,
                "Employee");

            return await base.UpsertAsync(dto);
        }
        public async Task<PagedList<RequestEmployeeResponseDto>> GetAllRequestEmployeeWithDetailsAsync(PageParam? pageParam)
        {
            Expression<Func<RequesterEmployee, object>>[] includes =
            {
               x => x.Employee
            };
            var effectivePageParam = pageParam ?? new PageParam();
            var entities = (await GetAllAsync(
                filter: null, // add filter if needed
                includes: includes,
                pageParam: effectivePageParam
            )).ToList();

            var dtos = _mapper.Map<IEnumerable<RequestEmployeeResponseDto>>(entities);

            return new PagedList<RequestEmployeeResponseDto>(
                dtos,
                effectivePageParam
             );
        }
    }
}
