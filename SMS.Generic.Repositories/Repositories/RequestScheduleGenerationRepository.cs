using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    public class RequestScheduleGenerationRepository
        : GenericRepository<RequestScheduleGeneration, SMSDbContext>, IRequestScheduleGenerationRepository
    {
        public RequestScheduleGenerationRepository(
            SMSDbContext context,
            ILogger<RequestScheduleGenerationRepository> logger) : base(context, logger)
        {
        }
    }
}
