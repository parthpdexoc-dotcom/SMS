using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

using SMS.Generic.Models.Constants;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing invoice equipment list operations using the generic service pattern.
    /// </summary>
    public class InvoiceEquipmentListService : GenericService<InvoiceEquipmentListDto, InvoiceEquipmentList>, IInvoiceEquipmentListService
    {
        private readonly IInvoiceEquipmentListRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceEquipmentListService"/> class.
        /// </summary>
        /// <param name="repository">The invoice equipment list repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public InvoiceEquipmentListService(
            IInvoiceEquipmentListRepository repository,
            IMapper mapper,
            ILogger<InvoiceEquipmentListService> logger) : base(repository, mapper, "InvoiceEquipmentListID")
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves an invoice equipment list by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The invoice equipment list DTO if found; otherwise, null.</returns>
        public async Task<InvoiceEquipmentListDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "InvoiceEquipmentListID");
            if (entity == null)
                return null;

            return _mapper.Map<InvoiceEquipmentListDto>(entity);
        }
    }
}

