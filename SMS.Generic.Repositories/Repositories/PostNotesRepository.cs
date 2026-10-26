using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    public class PostNotesRepository : GenericRepository<PostNotes, SMSDbContext>, IPostNotesRepository
    {
        public PostNotesRepository(
            SMSDbContext context,
            ILogger<GenericRepository<PostNotes, SMSDbContext>> logger)
            : base(context, logger)
        { }
    }
}
