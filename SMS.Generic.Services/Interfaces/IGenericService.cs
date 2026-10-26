using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using System.Linq.Expressions;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Generic service interface providing common business logic operations for DTOs and entities.
    /// </summary>
    /// <typeparam name="TDto">The DTO type.</typeparam>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    public interface IGenericService<TDto, TEntity> where TDto : class where TEntity : class
    {
        /// <summary>
        /// Retrieves all DTOs matching the specified filter with optional pagination and includes.
        /// </summary>
        /// <param name="filter">Optional filter expression to apply.</param>
        /// <param name="includes">Optional navigation properties to include in the query.</param>
        /// <param name="pageParam">Optional pagination parameters.</param>
        /// <returns>A collection of DTOs matching the criteria.</returns>
        Task<IEnumerable<TDto>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, Expression<Func<TEntity, object>>[] includes = null, PageParam pageParam = null);

        /// <summary>
        /// Gets the count of entities matching the specified filter with optional includes.
        /// Applies the same department/scope filters as other service methods when applicable.
        /// </summary>
        /// <param name="predicate">Optional predicate to filter entities.</param>
        /// <param name="includes">Optional navigation properties to include in the query.</param>
        /// <returns>The count of matching entities.</returns>
        Task<int> GetFilteredCountAsync(Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, object>>[] includes = null);

        /// <summary>
        /// Returns one page of DTOs and the total row count for the same filter by calling
        /// <see cref="GetFilteredCountAsync"/> then <see cref="GetAllAsync"/> (same scoping and includes).
        /// </summary>
        /// <param name="filter">Optional filter expression.</param>
        /// <param name="pageParam">Paging and sort; when null, a default <see cref="PageParam"/> is used.</param>
        /// <param name="includes">Optional navigation includes.</param>
        /// <returns>A paged list; <c>TotalRecords</c> is the filtered count before paging.</returns>
        Task<PagedList<TDto>> GetAllPagedAsync(Expression<Func<TEntity, bool>>? filter = null, PageParam pageParam = null, Expression<Func<TEntity, object>>[] includes = null);

        /// <summary>
        /// Performs paged search using a caller-provided predicate builder and optional base filter.
        /// </summary>
        /// <param name="searchTerm">The text to search for.</param>
        /// <param name="baseFilter">Optional base filter applied regardless of search term.</param>
        /// <param name="searchPredicateBuilder">Function that builds a predicate for the provided search term.</param>
        /// <param name="pageParam">Optional paging/sorting parameters.</param>
        /// <param name="includes">Optional navigation includes.</param>
        /// <returns>Paged search result where total count reflects all matches before paging.</returns>
        Task<PagedList<TDto>> SearchPagedAsync(
            string? searchTerm,
            Expression<Func<TEntity, bool>>? baseFilter = null,
            Func<string, Expression<Func<TEntity, bool>>>? searchPredicateBuilder = null,
            PageParam? pageParam = null,
            Expression<Func<TEntity, object>>[]? includes = null);

        /// <summary>
        /// Performs paged search with case-insensitive contains matching on specified string properties.
        /// </summary>
        /// <param name="searchTerm">The text to search for.</param>
        /// <param name="pageParam">Optional paging/sorting parameters.</param>
        /// <param name="filter">Optional base filter that is combined with the search filter.</param>
        /// <param name="includes">Optional navigation properties to include in the query.</param>
        /// <param name="searchableProperties">Entity string property names to search with case-insensitive contains.</param>
        /// <returns>Paged search result where <c>TotalRecords</c> reflects all matches before paging.</returns>
        Task<PagedList<TDto>> SearchAsync(
            string? searchTerm,
            PageParam? pageParam = null,
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, object>>[]? includes = null,
            params string[] searchableProperties);

        /// <summary>
        /// Returns id/label dropdown options using the entity primary key and a string label property.
        /// </summary>
        /// <param name="labelSelector">Entity string property used for the dropdown label.</param>
        /// <param name="filter">Optional filter (e.g. active status); department scope is applied by default.</param>
        /// <param name="searchTerm">Optional text search before the query runs.</param>
        /// <param name="searchSelectors">Optional extra entity properties to search; defaults to the label property.</param>
        Task<IEnumerable<SelectOptionDto>> GetOptionsAsync(
            Expression<Func<TEntity, string>> labelSelector,
            Expression<Func<TEntity, bool>>? filter = null,
            string? searchTerm = null,
            params Expression<Func<TEntity, object>>[] searchSelectors);

        /// <summary>
        /// Retrieves a DTO by its primary key (Guid).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The DTO if found; otherwise, null.</returns>
        Task<TDto> GetByIdAsync(Guid id);

        /// <summary>
        /// Retrieves a DTO by its primary key (string).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The DTO if found; otherwise, null.</returns>
        Task<TDto> GetByIdAsync(string id);

        Task<TDto?> GetByIdAsync(Guid id,Expression<Func<TEntity, object>> include);

        /// <summary>
        /// Retrieves a DTO by composite keys.
        /// </summary>
        /// <param name="key1">The first key value.</param>
        /// <param name="key1PropertyName">The name of the first key property.</param>
        /// <param name="key2">The second key value.</param>
        /// <param name="key2PropertyName">The name of the second key property.</param>
        /// <returns>The DTO if found; otherwise, null.</returns>
        Task<TDto> GetByCompositeKeysAsync(string key1, string key1PropertyName, string key2, string key2PropertyName);

        /// <summary>
        /// Retrieves a collection of DTOs by composite keys.
        /// </summary>
        /// <param name="key1">The first key value.</param>
        /// <param name="key1PropertyName">The name of the first key property.</param>
        /// <param name="key2">The second key value.</param>
        /// <param name="key2PropertyName">The name of the second key property.</param>
        /// <returns>A collection of DTOs matching the composite keys.</returns>
        Task<IEnumerable<TDto>> GetListByCompositeKeysAsync(string key1, string key1PropertyName, string key2, string key2PropertyName);

        /// <summary>
        /// Inserts a new DTO or updates an existing one.
        /// </summary>
        /// <param name="dto">The DTO to insert or update.</param>
        /// <param name="isSuperUser">
        /// When true, bypasses automatic audit field population (Created/Edited tracking).
        /// Defaults to false.
        /// </param>
        /// <returns>The upserted DTO.</returns>
        Task<TDto> UpsertAsync(TDto dto, bool isSuperUser = false);

        /// <summary>
        /// Inserts multiple new DTOs or updates existing ones.
        /// </summary>
        /// <param name="dtos">The DTOs to insert or update.</param>
        /// <param name="isSuperUser">
        /// When true, bypasses automatic audit field population (Created/Edited tracking) for each DTO, matching
        /// <see cref="UpsertAsync(TDto, bool)"/>.
        /// </param>
        /// <returns>A collection of upserted DTOs.</returns>
        Task<IEnumerable<TDto>> BulkUpsertAsync(IEnumerable<TDto> dtos, bool isSuperUser = false);

        /// <summary>
        /// Updates the primary key value of an entity.
        /// </summary>
        /// <param name="oldKey">The current primary key value.</param>
        /// <param name="newKey">The new primary key value.</param>
        /// <param name="updateProperty">The property to update (optional).</param>
        /// <returns>True if the primary key was updated; otherwise, false.</returns>
        Task<bool> UpdatePrimaryKeyAsync(string oldKey, string newKey, string updateProperty = null);

        /// <summary>
        /// Updates entities matching the specified filter using the provided update action.
        /// </summary>
        /// <param name="filter">The filter expression to identify entities to update.</param>
        /// <param name="updateAction">The action to apply to each matching entity.</param>
        /// <returns>The number of entities updated.</returns>
        Task<int> UpdateAsync(Expression<Func<TEntity, bool>> filter, Action<TEntity> updateAction);

        /// <summary>
        /// Deletes an entity by its primary key.
        /// </summary>
        /// <typeparam name="TKey">The type of the primary key.</typeparam>
        /// <param name="id">The primary key value.</param>
        /// <returns>True if the entity was deleted; otherwise, false.</returns>
        Task<bool> DeleteAsync<TKey>(TKey id);

        /// <summary>
        /// Deletes multiple entities by their key values.
        /// </summary>
        /// <typeparam name="TKey">The type of the key values.</typeparam>
        /// <param name="ids">The key values of entities to delete.</param>
        /// <returns>True if any entities were deleted; otherwise, false.</returns>
        Task<bool> BulkDeleteAsync<TKey>(List<TKey> ids);

        /// <summary>
        /// Deletes entities by a specific property value.
        /// </summary>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="propertyName">The name of the property to filter by.</param>
        /// <param name="value">The value to match.</param>
        /// <returns>True if any entities were deleted; otherwise, false.</returns>
        Task<bool> DeleteByPropertyAsync<TValue>(string propertyName, TValue value);

        /// <summary>
        /// Adds multiple DTOs to the repository.
        /// </summary>
        /// <param name="clientDtos">The DTOs to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddRangeAsync(IEnumerable<TDto> clientDtos);

        /// <summary>
        /// Retrieves DTOs by multiple composite key combinations.
        /// </summary>
        /// <param name="compositeKeys">A collection of dictionaries containing key-value pairs for composite keys.</param>
        /// <returns>A collection of DTOs matching any of the composite key combinations.</returns>
        Task<IEnumerable<TDto>> GetListByCompositeKeyArrayAsync(IEnumerable<Dictionary<string, string>> compositeKeys);
        Task<TDto?> GetByIdAsync(Guid id, params Expression<Func<TEntity, object>>[] includes);
    }
}
