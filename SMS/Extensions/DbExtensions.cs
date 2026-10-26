using Microsoft.EntityFrameworkCore;
using SMS.Data;
using SMS.Generic.Common.Audit;
using SMS.Generic.Dtos.Interfaces.Audit;
using SMS.Generic.Repositories.Audit;
using SMS.Generic.Services.Services.Audit;
using SMS.Infrastructure.Audit;

namespace SMS.Infrastructure.Extensions
{
    public static class DbExtensions
    {
        public static IServiceCollection AddSmsDb(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Missing DefaultConnection");

            services.AddDbContext<SMSDbContext>((serviceProvider, options) =>
            {
                options.UseSqlServer(connectionString, sql =>
                {
                    sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });

                // Suppress PendingModelChangesWarning to allow manual migration application
                options.ConfigureWarnings(warnings =>
                    warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));

                if (serviceProvider.GetService<AuditSaveChangesInterceptor>() is { } auditInterceptor)
                    options.AddInterceptors(auditInterceptor);
            });

            return services;
        }

        /// <summary>
        /// Registers the audit database context, in-memory audit channel, HTTP user context, and background persistence worker.
        /// </summary>
        public static IServiceCollection AddSmsAudit(this IServiceCollection services, IConfiguration configuration)
        {
            var auditConnection = configuration.GetConnectionString("AuditConnection")
                ?? throw new InvalidOperationException("Missing AuditConnection");

            services.AddDbContext<AuditDbContext>(options =>
            {
                options.UseSqlServer(auditConnection, sql =>
                    sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null));
                options.ConfigureWarnings(warnings =>
                    warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
            });

            services.AddSingleton<AuditQueueService>();
            services.AddSingleton<IAuditQueueService>(sp => sp.GetRequiredService<AuditQueueService>());
            services.AddScoped<IAuditUserContext, HttpAuditUserContext>();
            services.AddScoped<IJobAuditRouting, JobAuditRouting>();
            services.AddScoped<AuditSaveChangesInterceptor>();
            services.AddHostedService<AuditBackgroundService>();

            return services;
        }
    }
}
