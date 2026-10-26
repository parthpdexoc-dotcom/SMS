using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service implementation for RequestType (formerly RequestRecurrence).
    /// </summary>
    public class RequestTypeService : GenericService<RequestTypeDto, RequestType>, IRequestTypeService
    {
        public RequestTypeService(
            IRequestTypeRepository repository,
            IMapper mapper,
            ITokenParser tokenParser,
            ILogger<RequestTypeService> logger) : base(repository, mapper, "RequestTypeID", tokenParser)
        {
        }
    }
}
