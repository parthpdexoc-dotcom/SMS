using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing employee skill operations using the generic service pattern.
    /// </summary>
    public class EmployeeSkillService : GenericService<EmployeeSkillDto, EmployeeSkill>, IEmployeeSkillService
    {
        private readonly IEmployeeSkillRepository _repository;
        private readonly IMapper _mapper;
        private readonly ITokenParser _tokenParser;
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeSkillService"/> class.
        /// </summary>
        /// <param name="repository">The employee skill repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public EmployeeSkillService(
            IEmployeeSkillRepository repository,
            IMapper mapper,
            ITokenParser tokenParser,
            ILogger<EmployeeSkillService> logger)
            : base(repository, mapper, "EmployeeSkillID", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
        }
    }
}

