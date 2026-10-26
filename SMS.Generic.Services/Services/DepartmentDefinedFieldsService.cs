using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for DepartmentEquipment operations.
    /// </summary>
    public class DepartmentDefinedFieldsService : GenericService<DepartmentDefinedFieldsDto, DepartmentDefinedFields>, IDepartmentDefinedFieldsService
    {
        private readonly IDepartmentDefinedFieldsRepository _departmentDefinedFieldsRepository;
        private readonly ITokenParser _tokenParser;

        public DepartmentDefinedFieldsService(
            IDepartmentDefinedFieldsRepository repository,
            IMapper mapper,
            ILogger<DepartmentDefinedFieldsService> logger,
            ITokenParser tokenParser)
            : base(repository, mapper, "DepartmentDefinedFieldID", tokenParser)
        {
            _departmentDefinedFieldsRepository = repository;
            _tokenParser = tokenParser;
        }

        /// <inheritdoc />
        public async Task<DepartmentDefinedFieldsDto?> GetByDepartmentIdAsync(Guid departmentId)
        {
            await EnsureExistsForDepartmentAsync(departmentId).ConfigureAwait(false);
            Expression<Func<DepartmentDefinedFields, bool>> filter = x => x.DepartmentID == departmentId;
            var items = await GetAllAsync(filter, null, null).ConfigureAwait(false);
            return items.FirstOrDefault();
        }

        /// <inheritdoc />
        public async Task EnsureExistsForDepartmentAsync(Guid departmentId)
        {
            var count = await _departmentDefinedFieldsRepository
                .GetFilteredCountAsync(x => x.DepartmentID == departmentId).ConfigureAwait(false);

            if (count == 0)
            {
                var definedFields = new DepartmentDefinedFields
                {
                    DepartmentID = departmentId
                };
                await _departmentDefinedFieldsRepository.UpsertAsync(definedFields, "DepartmentDefinedFieldID").ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public Task<bool> DeleteByDepartmentIdAsync(Guid departmentId)
        {
            return DeleteByPropertyAsync("DepartmentID", departmentId);
        }
    }
}


