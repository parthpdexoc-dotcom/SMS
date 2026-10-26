using SMS.Generic.Common.Implementations.Paginations;
using System.Linq.Expressions;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>
    /// Generic repository interface providing common data access operations for entities.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves all entities matching the specified filter with optional pagination and includes.
        /// </summary>
        /// <param name="filter">Optional filter expression to apply.</param>
        /// <param name="includes">Optional navigation properties to include in the query.</param>
        /// <param name="pageParam">Optional pagination parameters.</param>
        /// <returns>A collection of entities matching the criteria.</returns>
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, Expression<Func<T, object>>[] includes = null, PageParam pageParam = null);

        /// <summary>
        /// Retrieves an entity by its primary key with optional navigation property include.
        /// </summary>
        /// <typeparam name="TKey">The type of the primary key.</typeparam>
        /// <param name="id">The primary key value.</param>
        /// <param name="keyPropertyName">The name of the primary key property.</param>
        /// <param name="include">Optional navigation property to include.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        Task<T?> GetByIdAsync<TKey>(TKey id, string keyPropertyName, Expression<Func<T, object>>? include = null) where TKey : notnull;

        /// <summary>
        /// Retrieves an entity by composite keys with optional navigation property include.
        /// </summary>
        /// <typeparam name="TKey1">The type of the first key.</typeparam>
        /// <typeparam name="TKey2">The type of the second key.</typeparam>
        /// <param name="key1">The first key value.</param>
        /// <param name="key1PropertyName">The name of the first key property.</param>
        /// <param name="key2">The second key value.</param>
        /// <param name="key2PropertyName">The name of the second key property.</param>
        /// <param name="include">Optional navigation property to include.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        Task<T?> GetByCompositeKeysAsync<TKey1, TKey2>(TKey1 key1, string key1PropertyName, TKey2 key2, string key2PropertyName, Expression<Func<T, object>>? include = null) where TKey1 : notnull where TKey2 : notnull;

        /// <summary>
        /// Retrieves a collection of entities by composite keys with optional navigation property include.
        /// </summary>
        /// <typeparam name="TKey1">The type of the first key.</typeparam>
        /// <typeparam name="TKey2">The type of the second key.</typeparam>
        /// <param name="key1">The first key value.</param>
        /// <param name="key1PropertyName">The name of the first key property.</param>
        /// <param name="key2">The second key value.</param>
        /// <param name="key2PropertyName">The name of the second key property.</param>
        /// <param name="include">Optional navigation property to include.</param>
        /// <returns>A collection of entities matching the composite keys.</returns>
        Task<IEnumerable<T>> GetListByCompositeKeysAsync<TKey1, TKey2>(TKey1 key1, string key1PropertyName, TKey2 key2, string key2PropertyName, Expression<Func<T, object>>? include = null) where TKey1 : notnull where TKey2 : notnull;

        /// <summary>
        /// Inserts a new entity or updates an existing one based on the primary key.
        /// </summary>
        /// <param name="entity">The entity to insert or update.</param>
        /// <param name="keyPropertyName">The name of the primary key property.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpsertAsync(T entity, string keyPropertyName);

        /// <summary>
        /// Inserts multiple new entities or updates existing ones based on the primary key.
        /// </summary>
        /// <param name="entities">The entities to insert or update.</param>
        /// <param name="keyPropertyName">The name of the primary key property.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task BulkUpsertAsync(IEnumerable<T> entities, string keyPropertyName);

        /// <summary>
        /// Updates entities matching the specified filter using the provided update action.
        /// </summary>
        /// <param name="filter">The filter expression to identify entities to update.</param>
        /// <param name="updateAction">The action to apply to each matching entity.</param>
        /// <returns>The number of entities updated.</returns>
        Task<int> UpdateAsync(Expression<Func<T, bool>> filter, Action<T> updateAction);

        /// <summary>
        /// Deletes an entity by its primary key.
        /// </summary>
        /// <typeparam name="TKey">The type of the primary key.</typeparam>
        /// <param name="id">The primary key value.</param>
        /// <returns>True if the entity was deleted; otherwise, false.</returns>
        Task<bool> DeleteAsync<TKey>(TKey id);

        /// <summary>
        /// Adds multiple entities to the repository.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Retrieves entities by multiple composite key combinations.
        /// </summary>
        /// <param name="compositeKeys">A collection of dictionaries containing key-value pairs for composite keys.</param>
        /// <returns>A collection of entities matching any of the composite key combinations.</returns>
        Task<IEnumerable<T>> GetListByCompositeKeyArrayAsync(IEnumerable<Dictionary<string, string>> compositeKeys);

        /// <summary>
        /// Deletes multiple entities by their key values.
        /// </summary>
        /// <typeparam name="TKey">The type of the key values.</typeparam>
        /// <param name="keyValues">The key values of entities to delete.</param>
        /// <param name="keyPropertyName">The name of the key property.</param>
        /// <returns>True if any entities were deleted; otherwise, false.</returns>
        Task<bool> BulkDeleteAsync<TKey>(List<TKey> keyValues, string keyPropertyName);

        /// <summary>
        /// Updates the primary key value of an entity.
        /// </summary>
        /// <param name="oldKey">The current primary key value.</param>
        /// <param name="newKey">The new primary key value.</param>
        /// <param name="updateProperty">The property to update (optional).</param>
        /// <returns>True if the primary key was updated; otherwise, false.</returns>
        Task<bool> UpdatePrimaryKeyAsync(string oldKey, string newKey, string updateProperty = null);

        /// <summary>
        /// Deletes entities by a specific property value.
        /// </summary>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="propertyName">The name of the property to filter by.</param>
        /// <param name="value">The value to match.</param>
        /// <returns>True if any entities were deleted; otherwise, false.</returns>
        Task<bool> DeleteByPropertyAsync<TValue>(string propertyName, TValue value);

        /// <summary>
        /// Gets the count of entities matching the specified predicate with optional includes.
        /// </summary>
        /// <param name="predicate">Optional predicate to filter entities.</param>
        /// <param name="includes">Optional navigation properties to include in the query.</param>
        /// <returns>The count of matching entities.</returns>
        Task<int> GetFilteredCountAsync(Expression<Func<T, bool>>? predicate = null, Expression<Func<T, object>>[] includes = null);

        /// <summary>
        /// Projects entities to a lightweight shape without loading full rows or applying pagination.
        /// </summary>
        Task<IReadOnlyList<TResult>> GetOptionsAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<TResult, object>>? orderBy = null);

        Task<T?> GetByIdAsync<TKey>(TKey id, string keyPropertyName, params Expression<Func<T, object>>[] includes) where TKey : notnull;
    }
}
