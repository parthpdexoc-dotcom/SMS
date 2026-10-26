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
    /// Service for managing invoice equipment operations using the generic service pattern.
    /// </summary>
    public class InvoiceEquipmentService : GenericService<InvoiceEquipmentDto, InvoiceEquipment>, IInvoiceEquipmentService
    {
        private readonly IInvoiceEquipmentRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceEquipmentService"/> class.
        /// </summary>
        /// <param name="repository">The invoice equipment repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public InvoiceEquipmentService(
            IInvoiceEquipmentRepository repository,
            IMapper mapper,
            ILogger<InvoiceEquipmentService> logger) : base(repository, mapper, "InvoiceEquipmentID")
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves an invoice equipment by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The invoice equipment DTO if found; otherwise, null.</returns>
        public async Task<InvoiceEquipmentDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "InvoiceEquipmentID");
            if (entity == null)
                return null;

            return _mapper.Map<InvoiceEquipmentDto>(entity);
        }
    }
}

