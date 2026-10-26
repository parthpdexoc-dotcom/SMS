using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing equipment operations using the generic service pattern.
    /// </summary>
    public class EquipmentService : GenericService<EquipmentDto, Equipment>, IEquipmentService
    {
        private readonly IEquipmentRepository _repository;
        private readonly IMapper _mapper;
        private readonly ITokenParser _tokenParser;
        private readonly IEquipmentTypeService _equipmentTypeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentService"/> class.
        /// </summary>
        /// <param name="repository">The equipment repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="tokenParser">The token parser instance.</param>
        /// <param name="logger">The logger instance.</param>
        public EquipmentService(
            IEquipmentRepository repository,
            IMapper mapper,
            ITokenParser tokenParser,
            ILogger<EquipmentService> logger,
            IEquipmentTypeService equipmentTypeService) : base(repository, mapper, "EquipementID", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
            _tokenParser = tokenParser;
            _equipmentTypeService = equipmentTypeService;
        }

        /// <summary>
        /// Inserts a new equipment or updates an existing one with duplicate validation.
        /// </summary>
        /// <param name="dto">The equipment DTO to insert or update.</param>
        /// <returns>The upserted equipment DTO.</returns>
        public override async Task<EquipmentDto> UpsertAsync(EquipmentDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // Validate EquipementTypeID exists in database
            await ValidateForeignKeyAsync<IEquipmentTypeService, EquipmentTypeDto>(_equipmentTypeService, dto.EquipementTypeID, "EquipmentType");

            // Validate for duplicate EquipementShort within the same department
            if (!string.IsNullOrWhiteSpace(dto.EquipementShort))
            {
                await ValidateDuplicateAsync(
                    propertyName: "EquipementShort",
                    value: dto.EquipementShort,
                    dto: dto,
                    isDepartmentScoped: true,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_EQUIPMENT_SHORT_DEPARTMENT);
            }

            return await base.UpsertAsync(dto);
        }

    }
}

