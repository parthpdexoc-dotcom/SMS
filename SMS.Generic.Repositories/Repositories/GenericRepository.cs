using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SMS.Generic.Repositories
{
    /// <summary>
    /// Generic repository implementation providing common data access operations for entities.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <typeparam name="TContext">The database context type.</typeparam>
    public class GenericRepository<T, TContext> : IGenericRepository<T> where T : class where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly ILogger<GenericRepository<T, TContext>> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository{T, TContext}"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when context or logger is null.</exception>
        public GenericRepository(TContext context, ILogger<GenericRepository<T, TContext>> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository{T, TContext}"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <remarks>
        /// This constructor is provided for backward compatibility.
        /// Consider using the constructor that accepts ILogger for better observability.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public GenericRepository(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
            _logger = null; // Will be null for backward compatibility
        }

        /// <summary>
        /// Validates that a key property exists on the entity type.
        /// </summary>
        /// <param name="keyPropertyName">The name of the key property to validate.</param>
        /// <returns>The PropertyInfo for the key property.</returns>
        /// <exception cref="ArgumentException">Thrown when the property is not found or invalid.</exception>
        private PropertyInfo ValidateKeyProperty(string keyPropertyName)
        {
            if (string.IsNullOrWhiteSpace(keyPropertyName))
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REPOSITORY_KEY_PROPERTY_NAME_NULL_OR_EMPTY), nameof(keyPropertyName));

            var keyProperty = typeof(T).GetProperty(keyPropertyName);
            if (keyProperty == null)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REPOSITORY_PROPERTY_NOT_FOUND_ON_TYPE, keyPropertyName, typeof(T).Name));

            return keyProperty;
        }

        /// <summary>
        /// Applies eager-load includes. When more than one include is specified, uses a split query to avoid cartesian explosion.
        /// </summary>
        private static IQueryable<T> ApplyIncludes(IQueryable<T> query, Expression<Func<T, object>>[]? includes)
        {
            if (includes == null || includes.Length == 0)
                return query;

            foreach (var include in includes)
                query = query.Include(include);

            if (includes.Length > 1)
                query = query.AsSplitQuery();

            return query;
        }

        /// <summary>
        /// Logs an error with context information.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="methodName">The name of the method where the error occurred.</param>
        /// <param name="additionalInfo">Additional context information.</param>
        private void LogError(Exception ex, string methodName, string additionalInfo = null)
        {
            if (_logger != null)
            {
                var message = $"Error occurred in {methodName} for entity {typeof(T).Name}";
                if (!string.IsNullOrWhiteSpace(additionalInfo))
                    message += $": {additionalInfo}";
                
                _logger.LogError(ex, message);
            }
        }

        /// <summary>
        /// Logs a debug message with context information.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="args">Additional arguments for the message.</param>
        private void LogDebug(string message, params object[] args)
        {
            if (_logger != null)
            {
                _logger.LogDebug(message, args);
            }
        }
        /// <summary>
        /// Retrieves all entities matching the specified filter with optional pagination and includes.
        /// </summary>
        /// <param name="filter">Optional filter expression to apply.</param>
        /// <param name="includes">Optional navigation properties to include in the query.</param>
        /// <param name="pageParam">Optional pagination parameters.</param>
        /// <returns>A collection of entities matching the criteria.</returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, Expression<Func<T, object>>[] includes = null, PageParam pageParam = null)
        {
            LogDebug("Retrieving all entities of type {EntityType} with filter: {HasFilter}, includes: {IncludeCount}, pagination: {HasPagination}", 
                typeof(T).Name, filter != null, includes?.Length ?? 0, pageParam != null);

            try
            {
                IQueryable<T> query = _dbSet;

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                query = ApplyIncludes(query, includes);

                // Apply sorting if specified
                if (pageParam != null && !string.IsNullOrWhiteSpace(pageParam.SortBy))
                {
                    var sortBy = pageParam.SortBy;
                    var sortDirection = pageParam.SortDirection?.ToLower() ?? "asc";

                    // Create parameter expression for the entity type
                    var parameter = Expression.Parameter(typeof(T), "x");

                    // Create property access expression
                    var property = Expression.Property(parameter, sortBy);

                    // Create the lambda expression for sorting
                    var lambda = Expression.Lambda(property, parameter);

                    // Create the method call expression for OrderBy/OrderByDescending
                    var methodName = sortDirection == "desc" ? "OrderByDescending" : "OrderBy";
                    var methodCall = Expression.Call(
                        typeof(Queryable),
                        methodName,
                        new[] { typeof(T), property.Type },
                        query.Expression,
                        Expression.Quote(lambda));

                    query = query.Provider.CreateQuery<T>(methodCall);
                }

                if (pageParam != null)
                {
                    query = query.Skip(pageParam.Offset.Value).Take(pageParam.Limit.Value);
                }

                var result = await query.ToListAsync();
                LogDebug("Successfully retrieved {Count} entities of type {EntityType}", result.Count, typeof(T).Name);
                return result;
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(GetAllAsync), $"Filter: {filter != null}, Includes: {includes?.Length ?? 0}, Pagination: {pageParam != null}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves an entity by its primary key with optional navigation property include.
        /// </summary>
        /// <typeparam name="TKey">The type of the primary key.</typeparam>
        /// <param name="id">The primary key value.</param>
        /// <param name="keyPropertyName">The name of the primary key property.</param>
        /// <param name="include">Optional navigation property to include.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        /// <exception cref="ArgumentNullException">Thrown when id is null.</exception>
        /// <exception cref="ArgumentException">Thrown when keyPropertyName is invalid.</exception>
        public virtual async Task<T?> GetByIdAsync<TKey>(TKey id, string keyPropertyName, Expression<Func<T, object>>? include = null) where TKey : notnull
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            LogDebug("Retrieving entity of type {EntityType} by key {KeyPropertyName} with value {KeyValue}", 
                typeof(T).Name, keyPropertyName, id);

            try
            {
                var keyProperty = ValidateKeyProperty(keyPropertyName);
                IQueryable<T> query = _dbSet;

                if (include != null)
                {
                    query = query.Include(include);
                }

                var result = await query.FirstOrDefaultAsync(e => EF.Property<TKey>(e, keyPropertyName).Equals(id));
                LogDebug("Entity retrieval {(Found)} for type {EntityType} with key {KeyValue}", 
                    result != null ? "successful" : "unsuccessful", typeof(T).Name, id);
                return result;
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(GetByIdAsync), $"Key: {keyPropertyName}={id}");
                throw;
            }
        }
        public virtual async Task<T?> GetByIdAsync<TKey>(TKey id,string keyPropertyName,params Expression<Func<T, object>>[] includes) where TKey : notnull
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            IQueryable<T> query = _dbSet;

            query = ApplyIncludes(query, includes);

            var result = await query.FirstOrDefaultAsync(e =>
                EF.Property<TKey>(e, keyPropertyName).Equals(id));

            return result;
        }
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
        /// <exception cref="ArgumentNullException">Thrown when key1 or key2 is null.</exception>
        /// <exception cref="ArgumentException">Thrown when property names are invalid.</exception>
        public virtual async Task<T?> GetByCompositeKeysAsync<TKey1, TKey2>(TKey1 key1, string key1PropertyName, TKey2 key2, string key2PropertyName, Expression<Func<T, object>>? include = null) where TKey1 : notnull where TKey2 : notnull
        {
            if (key1 == null)
                throw new ArgumentNullException(nameof(key1));
            if (key2 == null)
                throw new ArgumentNullException(nameof(key2));

            LogDebug("Retrieving entity of type {EntityType} by composite keys {Key1PropertyName}={Key1Value}, {Key2PropertyName}={Key2Value}", 
                typeof(T).Name, key1PropertyName, key1, key2PropertyName, key2);

            try
            {
                var keyProperty1 = ValidateKeyProperty(key1PropertyName);
                var keyProperty2 = ValidateKeyProperty(key2PropertyName);
                IQueryable<T> query = _dbSet;

                if (include != null)
                {
                    query = query.Include(include);
                }

                var result = await query.FirstOrDefaultAsync(e =>
                    EF.Property<TKey1>(e, key1PropertyName)!.Equals(key1) &&
                    EF.Property<TKey2>(e, key2PropertyName)!.Equals(key2));

                LogDebug("Entity retrieval {(Found)} for type {EntityType} with composite keys", 
                    result != null ? "successful" : "unsuccessful", typeof(T).Name);
                return result;
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(GetByCompositeKeysAsync), $"Keys: {key1PropertyName}={key1}, {key2PropertyName}={key2}");
                throw;
            }
        }
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
        /// <exception cref="ArgumentNullException">Thrown when key1 or key2 is null.</exception>
        /// <exception cref="ArgumentException">Thrown when property names are invalid.</exception>
        public virtual async Task<IEnumerable<T>> GetListByCompositeKeysAsync<TKey1, TKey2>(TKey1 key1, string key1PropertyName, TKey2 key2, string key2PropertyName, Expression<Func<T, object>>? include = null) where TKey1 : notnull where TKey2 : notnull
        {
            if (key1 == null)
                throw new ArgumentNullException(nameof(key1));
            if (key2 == null)
                throw new ArgumentNullException(nameof(key2));

            LogDebug("Retrieving entities of type {EntityType} by composite keys {Key1PropertyName}={Key1Value}, {Key2PropertyName}={Key2Value}", 
                typeof(T).Name, key1PropertyName, key1, key2PropertyName, key2);

            try
            {
                var keyProperty1 = ValidateKeyProperty(key1PropertyName);
                var keyProperty2 = ValidateKeyProperty(key2PropertyName);
                IQueryable<T> query = _dbSet;

                if (include != null)
                {
                    query = query.Include(include);
                }

                var result = await query
                    .Where(e =>
                        EF.Property<TKey1>(e, key1PropertyName)!.Equals(key1) &&
                        EF.Property<TKey2>(e, key2PropertyName)!.Equals(key2))
                    .ToListAsync();

                LogDebug("Successfully retrieved {Count} entities of type {EntityType} with composite keys", 
                    result.Count, typeof(T).Name);
                return result;
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(GetListByCompositeKeysAsync), $"Keys: {key1PropertyName}={key1}, {key2PropertyName}={key2}");
                throw;
            }
        }

        /// <summary>
        /// Inserts a new entity or updates an existing one based on the primary key.
        /// </summary>
        /// <param name="entity">The entity to insert or update.</param>
        /// <param name="keyPropertyName">The name of the primary key property.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
        /// <exception cref="ArgumentException">Thrown when keyPropertyName is invalid.</exception>
        public virtual async Task UpsertAsync(T entity, string keyPropertyName)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            LogDebug("Upserting entity of type {EntityType} with key property {KeyPropertyName}", 
                typeof(T).Name, keyPropertyName);

            try
            {
                var keyProperty = ValidateKeyProperty(keyPropertyName);
                var keyValue = keyProperty.GetValue(entity);

                var entry = _context.Entry(entity);
                bool isNewEntity = false;
                
                if (entry.State == EntityState.Detached)
                {
                    var existingEntity = await _dbSet.FindAsync(keyValue);
                    if (existingEntity != null)
                    {
                        _context.Entry(existingEntity).CurrentValues.SetValues(entity);
                        LogDebug("Updated existing entity of type {EntityType} with key {KeyValue}", 
                            typeof(T).Name, keyValue);
                    }
                    else
                    {
                        await _dbSet.AddAsync(entity);
                        isNewEntity = true;
                        LogDebug("Added new entity of type {EntityType} with key {KeyValue}", 
                            typeof(T).Name, keyValue);
                    }
                }
                else
                {
                    entry.State = EntityState.Modified;
                    LogDebug("Modified tracked entity of type {EntityType}", typeof(T).Name);
                }

                await _context.SaveChangesAsync();
                
                // For new entities, reload to get the generated ID if it was database-generated
                if (isNewEntity && entry.State != EntityState.Detached)
                {
                    await entry.ReloadAsync();
                }
                
                LogDebug("Successfully saved changes for entity of type {EntityType}", typeof(T).Name);
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(UpsertAsync), $"Entity: {typeof(T).Name}, Key: {keyPropertyName}");
                throw;
            }
        }

        /// <summary>
        /// Inserts multiple new entities or updates existing ones based on the primary key.
        /// </summary>
        /// <param name="entities">The entities to insert or update.</param>
        /// <param name="keyPropertyName">The name of the primary key property.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when entities is null.</exception>
        /// <exception cref="ArgumentException">Thrown when keyPropertyName is invalid or entities is empty.</exception>
        public virtual async Task BulkUpsertAsync(IEnumerable<T> entities, string keyPropertyName)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var entitiesList = entities.ToList();
            if (!entitiesList.Any())
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REPOSITORY_ENTITIES_COLLECTION_EMPTY), nameof(entities));

            LogDebug("Bulk upserting {Count} entities of type {EntityType} with key property {KeyPropertyName}", 
                entitiesList.Count, typeof(T).Name, keyPropertyName);

            try
            {
                var keyProperty = ValidateKeyProperty(keyPropertyName);
                
                // Get all key values for bulk lookup
                var keyValues = entitiesList.Select(e => keyProperty.GetValue(e)).ToList();
                
                // Find existing entities in one query
                var existingEntities = await _dbSet
                    .Where(e => keyValues.Contains(EF.Property<object>(e, keyPropertyName)))
                    .ToListAsync();

                var existingKeys = existingEntities
                    .Select(e => keyProperty.GetValue(e))
                    .ToHashSet();

                var entitiesToAdd = new List<T>();
                var entitiesToUpdate = new List<T>();

                foreach (var entity in entitiesList)
                {
                    var keyValue = keyProperty.GetValue(entity);
                    var entry = _context.Entry(entity);

                    if (entry.State == EntityState.Detached)
                    {
                        if (existingKeys.Contains(keyValue))
                        {
                            var existingEntity = existingEntities.First(e => keyProperty.GetValue(e).Equals(keyValue));
                            var existingEntry = _context.Entry(existingEntity);
                            
                            // Preserve Created audit fields for existing records before updating
                            DateTime? preservedCreatedDate = null;
                            Guid? preservedCreatedUserID = null;
                            string? preservedCreatedBy = null;
                            
                            if (existingEntity is BaseEntity existingBaseEntity)
                            {
                                preservedCreatedDate = existingBaseEntity.CreatedDate;
                                preservedCreatedUserID = existingBaseEntity.CreatedUserID;
                                preservedCreatedBy = existingBaseEntity.CreatedBy;
                            }
                            
                            // Update all values from the new entity
                            existingEntry.CurrentValues.SetValues(entity);
                            
                            // Restore preserved Created fields after SetValues
                            if (existingEntity is BaseEntity baseEntity && 
                                (preservedCreatedDate.HasValue || preservedCreatedUserID.HasValue || preservedCreatedBy != null))
                            {
                                baseEntity.CreatedDate = preservedCreatedDate;
                                baseEntity.CreatedUserID = preservedCreatedUserID;
                                baseEntity.CreatedBy = preservedCreatedBy;
                            }
                            
                            entitiesToUpdate.Add(existingEntity); // Use existingEntity, not entity
                        }
                        else
                        {
                            entitiesToAdd.Add(entity);
                        }
                    }
                    else
                    {
                        entry.State = EntityState.Modified;
                        entitiesToUpdate.Add(entity);
                    }
                }

                if (entitiesToAdd.Any())
                {
                    await _dbSet.AddRangeAsync(entitiesToAdd);
                    LogDebug("Added {Count} new entities of type {EntityType}", entitiesToAdd.Count, typeof(T).Name);
                }

                if (entitiesToUpdate.Any())
                {
                    LogDebug("Updated {Count} existing entities of type {EntityType}", entitiesToUpdate.Count, typeof(T).Name);
                }

                await _context.SaveChangesAsync();
                LogDebug("Successfully saved bulk changes for {Count} entities of type {EntityType}", 
                    entitiesList.Count, typeof(T).Name);
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(BulkUpsertAsync), $"Entity: {typeof(T).Name}, Count: {entitiesList.Count}, Key: {keyPropertyName}");
                throw;
            }
        }
        /// <summary>
        /// Updates entities matching the specified filter using the provided update action.
        /// </summary>
        /// <param name="filter">The filter expression to identify entities to update.</param>
        /// <param name="updateAction">The action to apply to each matching entity.</param>
        /// <returns>The number of entities updated.</returns>
        /// <exception cref="ArgumentNullException">Thrown when filter or updateAction is null.</exception>
        public virtual async Task<int> UpdateAsync(Expression<Func<T, bool>> filter, Action<T> updateAction)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            if (updateAction == null)
                throw new ArgumentNullException(nameof(updateAction));

            LogDebug("Updating entities of type {EntityType} with custom filter", typeof(T).Name);

            try
            {
                var records = await _dbSet.Where(filter).ToListAsync();

                if (records.Count == 0)
                {
                    LogDebug("No entities found to update for type {EntityType}", typeof(T).Name);
                    return 0;
                }

                foreach (var record in records)
                {
                    updateAction(record);
                }

                var result = await _context.SaveChangesAsync();
                LogDebug("Successfully updated {Count} entities of type {EntityType}", result, typeof(T).Name);
                return result;
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(UpdateAsync), $"Entity: {typeof(T).Name}");
                throw;
            }
        }

        /// <summary>
        /// Deletes an entity by its primary key.
        /// </summary>
        /// <typeparam name="TKey">The type of the primary key.</typeparam>
        /// <param name="id">The primary key value.</param>
        /// <returns>True if the entity was deleted; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when id is null.</exception>
        public virtual async Task<bool> DeleteAsync<TKey>(TKey id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            LogDebug("Deleting entity of type {EntityType} with key {KeyValue}", typeof(T).Name, id);

            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    await _context.SaveChangesAsync();
                    LogDebug("Successfully deleted entity of type {EntityType} with key {KeyValue}", typeof(T).Name, id);
                    return true;
                }

                LogDebug("Entity not found for deletion: {EntityType} with key {KeyValue}", typeof(T).Name, id);
                return false;
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(DeleteAsync), $"Entity: {typeof(T).Name}, Key: {id}");
                throw;
            }
        }
        /// <summary>
        /// Deletes multiple entities by their key values.
        /// </summary>
        /// <typeparam name="TKey">The type of the key values.</typeparam>
        /// <param name="keyValues">The key values of entities to delete.</param>
        /// <param name="keyPropertyName">The name of the key property.</param>
        /// <returns>True if any entities were deleted; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when keyValues is null.</exception>
        /// <exception cref="ArgumentException">Thrown when keyPropertyName is invalid or keyValues is empty.</exception>
        public virtual async Task<bool> BulkDeleteAsync<TKey>(List<TKey> keyValues, string keyPropertyName)
        {
            if (keyValues == null)
                throw new ArgumentNullException(nameof(keyValues));
            if (!keyValues.Any())
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REPOSITORY_KEY_VALUES_COLLECTION_EMPTY), nameof(keyValues));

            LogDebug("Bulk deleting entities of type {EntityType} with key property {KeyPropertyName} for {Count} keys", 
                typeof(T).Name, keyPropertyName, keyValues.Count);

            try
            {
                ValidateKeyProperty(keyPropertyName);

                // Convert keyValues to List<object> and use EF.Property<object> like BulkUpsertAsync does
                var keyValuesAsObjects = keyValues.Cast<object>().ToList();
                
                var entities = await _dbSet
                    .Where(e => keyValuesAsObjects.Contains(EF.Property<object>(e, keyPropertyName)))
                    .ToListAsync();

                if (!entities.Any())
                {
                    LogDebug("No entities found for bulk deletion: {EntityType} with {Count} keys", 
                        typeof(T).Name, keyValues.Count);
                    return false;
                }

                _dbSet.RemoveRange(entities);
                await _context.SaveChangesAsync();

                LogDebug("Successfully bulk deleted {Count} entities of type {EntityType}", 
                    entities.Count, typeof(T).Name);
                return true;
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(BulkDeleteAsync), $"Entity: {typeof(T).Name}, Count: {keyValues.Count}, Key: {keyPropertyName}");
                throw;
            }
        }
        public async Task<bool> UpdatePrimaryKeyAsync(string oldKey, string newKey, string updateProperty = null)
        {
            var entityType = _context.Model.FindEntityType(typeof(T));
            var keyProperty = updateProperty ?? entityType.FindPrimaryKey().Properties.FirstOrDefault().Name;

            if (keyProperty == null)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REPOSITORY_NO_PRIMARY_KEY_DEFINED));

            var dbSet = _context.Set<T>();
            var parameter = Expression.Parameter(typeof(T), "e");

            // Build the lambda expression: e => e.Key == oldKey
            var property = Expression.Property(parameter, keyProperty);
            var oldKeyConstant = Expression.Constant(oldKey);
            var equalExpression = Expression.Equal(property, Expression.Convert(oldKeyConstant, property.Type));

            var lambda = Expression.Lambda<Func<T, bool>>(equalExpression, parameter);

            var entity = await dbSet.FirstOrDefaultAsync(lambda);

            if (entity == null)
                return false; // Not found

            // Set new primary key using reflection
            typeof(T).GetProperty(keyProperty)?.SetValue(entity, Convert.ChangeType(newKey, property.Type));

            _context.Update(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Adds multiple entities to the repository.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when entities is null.</exception>
        /// <exception cref="ArgumentException">Thrown when entities is empty.</exception>
        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var entitiesList = entities.ToList();
            if (!entitiesList.Any())
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REPOSITORY_ENTITIES_COLLECTION_EMPTY_WITH_ARTICLE), nameof(entities));

            LogDebug("Adding {Count} entities of type {EntityType}", entitiesList.Count, typeof(T).Name);

            try
            {
                await _dbSet.AddRangeAsync(entitiesList);
                await _context.SaveChangesAsync();
                LogDebug("Successfully added {Count} entities of type {EntityType}", entitiesList.Count, typeof(T).Name);
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(AddRangeAsync), $"Entity: {typeof(T).Name}, Count: {entitiesList.Count}");
                throw;
            }
        }

        public virtual async Task<T> GetAsync(Dictionary<string, object> keyProperties)
        {
            if (keyProperties == null || keyProperties.Count == 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REPOSITORY_AT_LEAST_ONE_KEY_PROPERTY_REQUIRED));

            IQueryable<T> query = _dbSet;

            // Parameter for the expression: x =>
            var parameter = Expression.Parameter(typeof(T), "x");

            // Build predicate: x => x.Prop1 == val1 && x.Prop2 == val2 ...
            Expression predicate = null;

            foreach (var kvp in keyProperties)
            {
                var propertyInfo = typeof(T).GetProperty(kvp.Key);
                if (propertyInfo == null)
                    throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REPOSITORY_PROPERTY_NOT_FOUND_ON_TYPE, kvp.Key, typeof(T).Name));

                var member = Expression.Property(parameter, propertyInfo);
                var constant = Expression.Constant(kvp.Value);
                var equality = Expression.Equal(member, Expression.Convert(constant, propertyInfo.PropertyType));

                predicate = predicate == null ? equality : Expression.AndAlso(predicate, equality);
            }

            var lambda = Expression.Lambda<Func<T, bool>>(predicate, parameter);

            // Execute the query
            return await query.FirstOrDefaultAsync(lambda);
        }
        public virtual async Task<IEnumerable<T>> GetListByCompositeKeyArrayAsync(IEnumerable<Dictionary<string, string>> compositeKeys)
        {
            if (compositeKeys == null || !compositeKeys.Any())
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REPOSITORY_COMPOSITE_KEYS_NULL_OR_EMPTY));

            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");

            // Build a list of key match expressions (x => (x.Prop1 == val1 && x.Prop2 == val2))
            var keyMatchExpressions = new List<Expression>();

            foreach (var keySet in compositeKeys)
            {
                Expression keyMatch = null;

                foreach (var kvp in keySet)
                {
                    var propertyInfo = typeof(T).GetProperty(kvp.Key);
                    if (propertyInfo == null)
                        throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REPOSITORY_PROPERTY_NOT_FOUND_ON_TYPE, kvp.Key, typeof(T).Name));

                    var member = Expression.Property(parameter, propertyInfo);
                    var constant = Expression.Constant(kvp.Value);
                    var equal = Expression.Equal(member, Expression.Convert(constant, propertyInfo.PropertyType));

                    keyMatch = keyMatch == null ? equal : Expression.AndAlso(keyMatch, equal);
                }

                if (keyMatch != null)
                    keyMatchExpressions.Add(keyMatch);
            }

            // Combine all key matches with OR
            Expression finalPredicate = keyMatchExpressions.Aggregate((left, right) => Expression.OrElse(left, right));

            var lambda = Expression.Lambda<Func<T, bool>>(finalPredicate, parameter);

            return await _dbSet.Where(lambda).ToListAsync();
        }
        public virtual async Task<bool> DeleteByPropertyAsync<TValue>(string propertyName, TValue value)
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            var property = Expression.Call(
                typeof(EF),
                nameof(EF.Property),
                new[] { typeof(TValue) },
                parameter,
                Expression.Constant(propertyName)
            );

            var equals = Expression.Equal(
                property,
                Expression.Constant(value)
            );

            var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);

            var entities = await _dbSet.Where(lambda).ToListAsync();

            if (!entities.Any())
                return false;

            _dbSet.RemoveRange(entities);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<T>> GetOrSetCacheAsync<T>(IMemoryCache cache, string cacheKey, Func<Task<List<T>>> getDataFunc, int expirationMinutes = 5)
        {
            if (cache.TryGetValue(cacheKey, out List<T>? cachedData))
            {
                return cachedData;
            }

            var data = await getDataFunc();

            if (data.Any())
            {
                cache.Set(cacheKey, data, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expirationMinutes),
                    Priority = CacheItemPriority.Normal
                });
            }
            return data;
        }

        /// <summary>
        /// Gets the count of entities matching the specified predicate with optional includes.
        /// </summary>
        /// <param name="predicate">Optional predicate to filter entities.</param>
        /// <param name="includes">Optional navigation properties to include in the query.</param>
        /// <returns>The count of matching entities.</returns>
        public virtual async Task<int> GetFilteredCountAsync(Expression<Func<T, bool>>? predicate = null, Expression<Func<T, object>>[] includes = null)
        {
            LogDebug("Getting filtered count for entities of type {EntityType} with predicate: {HasPredicate}, includes: {IncludeCount}", 
                typeof(T).Name, predicate != null, includes?.Length ?? 0);

            try
            {
                IQueryable<T> query = _dbSet;

                if (predicate != null)
                    query = query.Where(predicate);

                query = ApplyIncludes(query, includes);

                var count = await query.CountAsync();
                LogDebug("Found {Count} entities of type {EntityType} matching criteria", count, typeof(T).Name);
                return count;
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(GetFilteredCountAsync), $"Entity: {typeof(T).Name}, Predicate: {predicate != null}, Includes: {includes?.Length ?? 0}");
                throw;
            }
        }

        /// <inheritdoc />
        public virtual async Task<IReadOnlyList<TResult>> GetOptionsAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<TResult, object>>? orderBy = null)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            try
            {
                IQueryable<T> query = _dbSet.AsNoTracking();

                if (filter != null)
                    query = query.Where(filter);

                var projected = query.Select(selector);

                if (orderBy != null)
                    projected = projected.OrderBy(orderBy);

                return await projected.ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(GetOptionsAsync), $"Entity: {typeof(T).Name}, Result: {typeof(TResult).Name}");
                throw;
            }
        }
    }
}
