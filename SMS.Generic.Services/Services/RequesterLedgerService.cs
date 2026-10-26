using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services
{
    public class RequesterLedgerService : GenericService<RequesterLedgerDto, RequesterLedger>, IRequesterLedgerService
    {
        private readonly IRequesterLedgerRepository _repository;
        private readonly IRequesterService _requesterService;
        private readonly IMapper _mapper;
        private readonly ITokenParser _tokenParser;

        public RequesterLedgerService(
            IRequesterLedgerRepository repository,
            IRequesterService requesterService,
            IMapper mapper,
            ILogger<RequesterLedgerService> logger,
            ITokenParser tokenParser) : base(repository, mapper, "RequesterLedgerID", tokenParser)
        {
            _repository = repository;
            _requesterService = requesterService;
            _mapper = mapper;
            _tokenParser = tokenParser;
        }

        /// <inheritdoc />
        public async Task<RequesterLedgerDto> AddTransactionOfRequesterLedgerAsync(RequesterLedgerDto entry,CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entry);

            if (entry.RequesterID == Guid.Empty)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_ID));

            if (entry.Amount <= 0m)
                throw new InvalidOperationException("Ledger amount must be greater than zero.");

            var departmentIdFromToken = _tokenParser.GetDepartmentID();
            if (!departmentIdFromToken.HasValue || departmentIdFromToken.Value == Guid.Empty)
                throw new UnauthorizedAccessException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_DEPARTMENT_ID_MISSING_FROM_TOKEN));

            entry.DepartmentID = departmentIdFromToken.Value;

            var latestEntry = await _repository.GetLatestLedgerEntryByRequesterIdAsync(entry.RequesterID, cancellationToken: cancellationToken).ConfigureAwait(false);

            var requesterName = latestEntry?.Requester?.Name ?? entry.RequesterName;
            if (string.IsNullOrWhiteSpace(requesterName))
            {
                var requester = await _requesterService.GetByIdAsync(entry.RequesterID).ConfigureAwait(false);
                requesterName = requester?.Name;
            }

            return await UpsertAsync(entry.ApplySourceTransaction(latestEntry?.Balance ?? 0m, requesterName)).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<RequesterLedgerStatementDto> GetRequesterLedgerStatementAsync(Guid requesterId,DateTime? startDateTime = null,DateTime? endDateTime = null,            PageParam? pagination = null,CancellationToken cancellationToken = default)
        {
            var statementEntries = await _repository.GetLedgerEntriesForStatementAsync(requesterId, startDateTime, endDateTime, cancellationToken).ConfigureAwait(false);

            var entryDtos = _mapper.Map<List<RequesterLedgerDto>>(statementEntries);

            return new RequesterLedgerStatementDto(startDateTime,endDateTime,entryDtos,pagination ?? new PageParam());
        }

        /// <inheritdoc />
        public async Task<decimal> GetRequesterCurrentBalanceAsync(Guid requesterId,CancellationToken cancellationToken = default)
        {
            var latestEntry = await _repository.GetLatestLedgerEntryByRequesterIdAsync(requesterId, cancellationToken: cancellationToken).ConfigureAwait(false);
            return latestEntry?.Balance ?? 0m;
        }
    }
}
