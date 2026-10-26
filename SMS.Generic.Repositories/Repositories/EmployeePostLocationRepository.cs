using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository implementation for employee job-location readings.
    /// </summary>
    public class EmployeePostLocationRepository : GenericRepository<EmployeePostLocation, SMSDbContext>, IEmployeePostLocationRepository
    {
        private readonly IMapper _mapper;

        public EmployeePostLocationRepository(
            SMSDbContext context,
            ILogger<EmployeePostLocationRepository> logger,
            IMapper mapper)
            : base(context, logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc />
        public async Task<List<EmployeePostLocationDto>> GetCoordinatesByEmployeeIdAsync(
            Guid employeeId,
            Guid postPositionId,
            CancellationToken cancellationToken = default)
        {
            if (employeeId == Guid.Empty || postPositionId == Guid.Empty)
                return new List<EmployeePostLocationDto>();

            var entities = await GetAllAsync(
                x => x.PostPositionID == postPositionId && x.EmployeeID == employeeId).ConfigureAwait(false);

            return _mapper.Map<List<EmployeePostLocationDto>>(entities.OrderBy(x => x.CapturedAt));
        }
    }
}
