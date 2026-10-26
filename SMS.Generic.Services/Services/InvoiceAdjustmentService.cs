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
    /// Service for managing invoice adjustment operations using the generic service pattern.
    /// </summary>
    public class InvoiceAdjustmentService : GenericService<InvoiceAdjustmentDto, InvoiceAdjustment>, IInvoiceAdjustmentService
    {
        private readonly IInvoiceAdjustmentRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceAdjustmentService"/> class.
        /// </summary>
        /// <param name="repository">The invoice adjustment repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public InvoiceAdjustmentService(
            IInvoiceAdjustmentRepository repository,
            IMapper mapper,
            ILogger<InvoiceAdjustmentService> logger) : base(repository, mapper, "AdjustmentId")
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves an invoice adjustment by its primary key (long).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The invoice adjustment DTO if found; otherwise, null.</returns>
        public async Task<InvoiceAdjustmentDto?> GetByIdAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_MUST_BE_GREATER_THAN_ZERO), nameof(id));

            var entity = await _repository.GetByIdAsync(id, "AdjustmentId");
            if (entity == null)
                return null;

            return _mapper.Map<InvoiceAdjustmentDto>(entity);
        }
    }
}

