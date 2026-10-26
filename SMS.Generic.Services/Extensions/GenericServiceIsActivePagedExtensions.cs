using System.Linq.Expressions;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Extensions
{
    /// <summary>
    /// Common <see cref="bool?"/> <c>isActive</c> dispatch for paged list queries (active-only / inactive-only / no status filter).
    /// </summary>
    public static class GenericServiceIsActivePagedExtensions
    {
        /// <summary>
        /// When <paramref name="isActive"/> is true, uses <paramref name="activeFilter"/>; when false, <paramref name="inactiveFilter"/>; when null, no extra filter.
        /// </summary>
        public static Task<PagedList<TDto>> GetAllPagedAsyncWithIsActive<TDto, TEntity>(
            this IGenericService<TDto, TEntity> service,
            bool? isActive,
            PageParam? pagination,
            Expression<Func<TEntity, bool>> activeFilter,
            Expression<Func<TEntity, bool>> inactiveFilter)
            where TDto : class
            where TEntity : class
        {
            Expression<Func<TEntity, bool>>? filter = isActive switch
            {
                true => activeFilter,
                false => inactiveFilter,
                _ => null,
            };

            return service.GetAllPagedAsync(filter, pagination);
        }
    }
}
