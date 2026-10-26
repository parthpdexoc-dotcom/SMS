using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories;
using SMS.Generic.Repositories.Interfaces;
    public class RequestTypeRepository : GenericRepository<RequestType, SMSDbContext>, IRequestTypeRepository
    {
        public RequestTypeRepository(SMSDbContext context) : base(context)
        {
        }
    }
