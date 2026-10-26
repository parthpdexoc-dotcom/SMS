using Microsoft.EntityFrameworkCore;

namespace SMS.Data
{
    public static class DbContextTransactionExtensions
    {
        public static Task ExecuteInTransactionAsync(
            this SMSDbContext dbContext,
            Func<Task> action,
            CancellationToken cancellationToken = default)
        {
            return dbContext.ExecuteInTransactionAsync(async () =>
            {
                await action().ConfigureAwait(false);
                return true;
            }, cancellationToken);
        }

        public static async Task<T> ExecuteInTransactionAsync<T>(
            this SMSDbContext dbContext,
            Func<Task<T>> action,
            CancellationToken cancellationToken = default)
        {
            var strategy = dbContext.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                var transaction = dbContext.Database.CurrentTransaction;
                var ownsTransaction = transaction == null;
                if (ownsTransaction)
                    transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

                try
                {
                    var result = await action().ConfigureAwait(false);
                    if (ownsTransaction)
                        await transaction!.CommitAsync(cancellationToken).ConfigureAwait(false);
                    return result;
                }
                catch
                {
                    if (ownsTransaction)
                        await transaction!.RollbackAsync(cancellationToken).ConfigureAwait(false);
                    throw;
                }
                finally
                {
                    if (ownsTransaction)
                        await transaction!.DisposeAsync().ConfigureAwait(false);
                }
            }).ConfigureAwait(false);
        }
    }
}
