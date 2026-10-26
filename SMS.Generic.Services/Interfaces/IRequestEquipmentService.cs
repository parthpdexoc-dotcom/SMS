using System;
using System.Collections.Generic;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for request equipment service operations.
    /// </summary>
    public interface IRequestEquipmentService : IGenericService<RequestEquipmentDto, RequestEquipment>
    {
        Task<IReadOnlyList<RequestEquipmentWithNameDto>> GetByRequestIdWithNamesAsync(Guid requestId, PageParam? pagination = null);
    }
}

