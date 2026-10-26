using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SMS.Generic.Common;
using SMS.Generic.Common.Constants;
using SMS.Generic.Common.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Interfaces;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using SMS.Logger.Logger;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;

namespace SMS.Generic.Services
{
    /// <summary>
    /// Generic service implementation providing common business logic operations for DTOs and entities.
    /// </summary>
    /// <typeparam name="TDto">The DTO type.</typeparam>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    public abstract class GenericService<TDto, TEntity> : IGenericService<TDto, TEntity> where TEntity : class where TDto : class
    {
        private readonly IGenericRepository<TEntity> _repository;
        private readonly IMapper _mapper;
        private readonly string _primaryKeyName;
        private readonly ISMSLogger _logger;
        private readonly ITokenParser? _tokenParser;
        private Guid? departmentId => _tokenParser.GetDepartmentID();
        private Guid? employeeId => _tokenParser.GetEmployeeID();
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericService{TDto, TEntity}"/> class.
        /// </summary>
        /// <param name="repository">The generic repository for data access.</param>
        /// <param name="mapper">The AutoMapper instance for object mapping.</param>
        /// <param name="primaryKeyName">The name of the primary key property.</param>
        /// <param name="tokenParser">Optional token parser for accessing current user information for audit tracking.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
        public GenericService(IGenericRepository<TEntity> repository, IMapper mapper, string primaryKeyName, ITokenParser? tokenParser = null)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _primaryKeyName = primaryKeyName ?? throw new ArgumentNullException(nameof(primaryKeyName));
            _tokenParser = tokenParser;
            _logger = SMSLogManager.GetLogger(typeof(GenericService<TDto, TEntity>));
        }

        /// <summary>
        /// Logs an error with context information.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="methodName">The name of the method where the error occurred.</param>
        /// <param name="additionalInfo">Additional context information.</param>
        private void LogError(Exception ex, string methodName, string additionalInfo = null)
        {
            var message = $"Error occurred in {methodName} for service {typeof(TEntity).Name}";
            if (!string.IsNullOrWhiteSpace(additionalInfo))
                message += $": {additionalInfo}";
            
            _logger.Error(message, ex);
        }

        /// <summary>
        /// Logs an informational message with formatted parameters.
        /// </summary>
        /// <param name="message">The message format string.</param>
        /// <param name="args">The arguments to format into the message.</param>
        private void LogInfo(string message, params object[] args)
        {
            if (args?.Length > 0)
                _logger.InfoFormat(message, memberName: "", lineNumber: 0, args);
            else
                _logger.Info(message);
        }

        /// <summary>
        /// Logs a debug message with formatted parameters.
        /// </summary>
        /// <param name="message">The message format string.</param>
        /// <param name="args">The arguments to format into the message.</param>
        private void LogDebug(string message, params object[] args)
        {
            if (args?.Length > 0)
                _logger.InfoFormat($"[DEBUG] {message}", memberName: "", lineNumber: 0, args);
            else
                _logger.Info($"[DEBUG] {message}");
        }

        /// <summary>
        /// Retrieves all DTOs matching the specified filter with optional pagination and includes.
        /// </summary>
        /// <param name="filter">Optional filter expression to apply.</param>
        /// <param name="includes">Optional navigation properties to include in the query.</param>
        /// <param name="pageParam">Optional pagination parameters.</param>
        /// <returns>A collection of DTOs matching the criteria.</returns>
        /// <exception cref="ArgumentException">Thrown when includes array is empty.</exception>
        public async virtual Task<IEnumerable<TDto>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, Expression<Func<TEntity, object>>[] includes = null, PageParam pageParam = null)
        {
            if (includes != null && includes.Length == 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_INCLUDES_ARRAY_EMPTY), nameof(includes));

            LogDebug("Retrieving all DTOs of type {0} with filter: {1}, includes: {2}, pagination: {3}", 
                typeof(TDto).Name, filter != null, includes?.Length ?? 0, pageParam != null);

            try
            {
                var finalFilter = AppendDepartmentFilter(filter);

                var entities = await _repository.GetAllAsync(finalFilter, includes, pageParam).ConfigureAwait(false);
                var result = _mapper.Map<IEnumerable<TDto>>(entities);
                
                LogDebug("Successfully retrieved {0} DTOs of type {1}", 
                    result.Count(), typeof(TDto).Name);
                return result;
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(GetAllAsync), $"Filter: {filter != null}, Includes: {includes?.Length ?? 0}, Pagination: {pageParam != null}");
                throw;
            }
        }

        /// <summary>
        /// Gets the count of entities matching the specified filter with optional includes.
        /// Applies the same department/scope filters as other service methods when applicable.
        /// </summary>
        /// <param name="predicate">Optional predicate to filter entities.</param>
        /// <param name="includes">Optional navigation properties to include in the query.</param>
        /// <returns>The count of matching entities.</returns>
        public async virtual Task<int> GetFilteredCountAsync(Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, object>>[] includes = null)
        {
            LogDebug("Getting filtered count for type {0}, predicate: {1}, includes: {2}",
                typeof(TEntity).Name, predicate != null, includes?.Length ?? 0);

            try
            {
                var finalFilter = AppendDepartmentFilter(predicate);
                var count = await _repository.GetFilteredCountAsync(finalFilter, includes).ConfigureAwait(false);
                LogDebug("Filtered count for type {0}: {1}", typeof(TEntity).Name, count);
                return count;
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(GetFilteredCountAsync), $"Predicate: {predicate != null}, Includes: {includes?.Length ?? 0}");
                throw;
            }
        }

        /// <inheritdoc />l
        public async virtual Task<PagedList<TDto>> GetAllPagedAsync(Expression<Func<TEntity, bool>>? filter = null, PageParam pageParam = null, Expression<Func<TEntity, object>>[] includes = null)
        {
            if (includes != null && includes.Length == 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_INCLUDES_ARRAY_EMPTY), nameof(includes));

            var effectivePageParam = pageParam ?? new PageParam();

            LogDebug("Retrieving paged DTOs of type {0} with filter: {1}, includes: {2}",
                typeof(TDto).Name, filter != null, includes?.Length ?? 0);

            try
            {
                var total = await GetFilteredCountAsync(filter, includes).ConfigureAwait(false);
                var items = await GetAllAsync(filter, includes, effectivePageParam).ConfigureAwait(false);
                return new PagedList<TDto>(items, effectivePageParam, total);
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(GetAllPagedAsync), $"Filter: {filter != null}, Includes: {includes?.Length ?? 0}");
                throw;
            }
        }

        /// <summary>
        /// Performs a text search across the provided string properties and returns paged results.
        /// </summary>
        /// <param name="searchTerm">Free-text search term. When empty, only the base filter is applied.</param>
        /// <param name="pageParam">Optional paging/sorting parameters.</param>
        /// <param name="filter">Optional base filter that is combined with the search filter.</param>
        /// <param name="includes">Optional navigation properties to include in the query.</param>
        /// <param name="searchableProperties">Entity string property names to search with case-insensitive contains.</param>
        /// <returns>Paged search result where <c>TotalRecords</c> reflects all matches before paging.</returns>
        public virtual async Task<PagedList<TDto>> SearchAsync(
            string? searchTerm,
            PageParam? pageParam = null,
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, object>>[]? includes = null,
            params string[] searchableProperties)
        {
            var effectivePageParam = pageParam ?? new PageParam();
            var effectiveFilter = BuildSearchFilter(searchTerm, filter, searchableProperties);
            return await GetAllPagedAsync(effectiveFilter, effectivePageParam, includes).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<SelectOptionDto>> GetOptionsAsync(
            Expression<Func<TEntity, string>> labelSelector,
            Expression<Func<TEntity, bool>>? filter = null,
            string? searchTerm = null,
            params Expression<Func<TEntity, object>>[] searchSelectors)
        {
            var labelPropertyName = GetPropertyName(labelSelector);
            var searchProperties = searchSelectors.Length > 0
                ? searchSelectors
                : new Expression<Func<TEntity, object>>[] { ToObjectSelector(labelSelector) };

            return await GetOptionsAsync(
                e => new SelectOptionDto
                {
                    Id = EF.Property<Guid>(e, _primaryKeyName),
                    Label = EF.Property<string>(e, labelPropertyName) ?? string.Empty
                },
                filter,
                searchTerm,
                searchProperties).ConfigureAwait(false);
        }

        protected virtual async Task<IEnumerable<SelectOptionDto>> GetOptionsAsync(Expression<Func<TEntity, SelectOptionDto>> selector,Expression<Func<TEntity, bool>>? filter = null,string? searchTerm = null,params Expression<Func<TEntity, object>>[] searchSelectors)
        {
            var finalFilter = BuildOptionsFilter(filter, searchTerm, searchSelectors);
            return await _repository.GetOptionsAsync(selector, finalFilter, o => o.Label).ConfigureAwait(false);
        }

        protected Expression<Func<TEntity, bool>>? BuildOptionsFilter(
            Expression<Func<TEntity, bool>>? filter,
            string? searchTerm,
            params Expression<Func<TEntity, object>>[] searchableProperties)
        {
            var finalFilter = AppendDepartmentFilter(filter);

            if (!string.IsNullOrWhiteSpace(searchTerm) && searchableProperties.Length > 0)
            {
                var propertyNames = searchableProperties.Select(GetPropertyName).ToArray();
                finalFilter = BuildSearchFilter(searchTerm, finalFilter, propertyNames);
            }

            return finalFilter;
        }

        private static Expression<Func<TEntity, object>> ToObjectSelector(Expression<Func<TEntity, string>> selector)
        {
            var parameter = selector.Parameters[0];
            var body = Expression.Convert(selector.Body, typeof(object));
            return Expression.Lambda<Func<TEntity, object>>(body, parameter);
        }

        private static string GetPropertyName(LambdaExpression expression)
        {
            var body = expression.Body;

            if (body is UnaryExpression { Operand: var operand })
                body = operand;

            if (body is MemberExpression { Member: PropertyInfo property })
                return property.Name;

            throw new ArgumentException("Expression must be a property access.", nameof(expression));
        }

        /// <inheritdoc />
        public virtual Task<PagedList<TDto>> SearchPagedAsync(
            string? searchTerm,
            Expression<Func<TEntity, bool>>? baseFilter,
            Func<string, Expression<Func<TEntity, bool>>> searchPredicateBuilder,
            PageParam? pageParam = null,
            Expression<Func<TEntity, object>>[]? includes = null)
        {
            if (searchPredicateBuilder == null)
                throw new ArgumentNullException(nameof(searchPredicateBuilder));

            Expression<Func<TEntity, bool>>? finalFilter = baseFilter;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchFilter = searchPredicateBuilder(searchTerm.Trim());
                finalFilter = finalFilter == null
                    ? searchFilter
                    : CombineFilters(finalFilter, searchFilter);
            }

            return GetAllPagedAsync(finalFilter, pageParam, includes);
        }

        private static Expression<Func<TEntity, bool>> CombineFilters(
            Expression<Func<TEntity, bool>> left,
            Expression<Func<TEntity, bool>> right)
        {
            var parameter = left.Parameters[0];
            var rightBody = new ReplaceParameterVisitor(right.Parameters[0], parameter).Visit(right.Body);
            var body = Expression.AndAlso(left.Body, rightBody!);
            return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
        }

        private static Expression<Func<TEntity, bool>> BuildSearchFilter(
            string? searchTerm,
            Expression<Func<TEntity, bool>>? filter,
            params string[] searchableProperties)
        {
            Expression<Func<TEntity, bool>> effectiveFilter = filter ?? (x => true);

            if (string.IsNullOrWhiteSpace(searchTerm))
                return effectiveFilter;

            if (searchableProperties == null || searchableProperties.Length == 0)
                throw new ArgumentException("At least one searchable property must be provided.", nameof(searchableProperties));

            var tokens = searchTerm
                .ToLower()
                .Replace(",", " ")
                .Split(" ", StringSplitOptions.RemoveEmptyEntries);

            var parameter = Expression.Parameter(typeof(TEntity), "x");

            var properties = searchableProperties
                .Distinct()
                .Select(name => typeof(TEntity).GetProperty(name))
                .Where(p => p != null && (p!.PropertyType == typeof(string) || Nullable.GetUnderlyingType(p.PropertyType) == typeof(string)))
                .ToList();

            if (properties.Count == 0)
                throw new ArgumentException("At least one valid string searchable property must be provided.", nameof(searchableProperties));

            var finalSearchBody = tokens
                .Select(token =>
                    properties
                        .Select(property =>
                        {
                            var propertyExpr = Expression.Property(parameter, property!);

                            var notNull = Expression.NotEqual(
                                propertyExpr,
                                Expression.Constant(null, typeof(string)));

                            var toLower = Expression.Call(
                                propertyExpr,
                                typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!);

                            var replace = Expression.Call(
                                toLower,
                                typeof(string).GetMethod(nameof(string.Replace), new[] { typeof(string), typeof(string) })!,
                                Expression.Constant(","),
                                Expression.Constant(""));

                            var contains = Expression.Call(
                                replace,
                                typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!,
                                Expression.Constant(token));

                            return Expression.AndAlso(notNull, contains);
                        })
                        .Aggregate(Expression.OrElse)
                )
                .Aggregate(Expression.AndAlso);

            var searchFilter = Expression.Lambda<Func<TEntity, bool>>(finalSearchBody, parameter);
            return CombineFilters(effectiveFilter, searchFilter);
        }

        /// <summary>
        /// Retrieves a DTO by its primary key (Guid).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The DTO if found; otherwise, null.</returns>
        /// <exception cref="ArgumentException">Thrown when id is empty.</exception>
        public async virtual Task<TDto> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_CANNOT_BE_EMPTY), nameof(id));

            LogDebug("Retrieving DTO of type {0} by Guid key {1}", typeof(TDto).Name, id);

            try
            {
                var entity = await _repository.GetByIdAsync(id, _primaryKeyName).ConfigureAwait(false);
                var result = _mapper.Map<TDto>(entity);
                
                LogDebug("DTO retrieval {0} for type {1} with key {2}", 
                    result != null ? "successful" : "unsuccessful", typeof(TDto).Name, id);
                return result;
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(GetByIdAsync), $"Guid: {id}");
                throw;
            }
        }
        public async virtual Task<TDto?> GetByIdAsync(Guid id, Expression<Func<TEntity, object>> include)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_CANNOT_BE_EMPTY));

            var entity = await _repository.GetByIdAsync(id, _primaryKeyName, include);

            return _mapper.Map<TDto>(entity);
        }
        public async virtual Task<TDto?> GetByIdAsync(Guid id,params Expression<Func<TEntity, object>>[] includes)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_CANNOT_BE_EMPTY));

            var entity = await _repository.GetByIdAsync(id,_primaryKeyName,includes);

            return _mapper.Map<TDto>(entity);
        }
        /// <summary>
        /// Retrieves a DTO by its primary key (string).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The DTO if found; otherwise, null.</returns>
        /// <exception cref="ArgumentException">Thrown when id is null or empty.</exception>
        public async virtual Task<TDto> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ID_NULL_OR_EMPTY), nameof(id));

            LogDebug("Retrieving DTO of type {0} by string key {1}", typeof(TDto).Name, id);

            try
            {
                var entity = await _repository.GetByIdAsync(id, _primaryKeyName).ConfigureAwait(false);
                var result = _mapper.Map<TDto>(entity);
                
                LogDebug("DTO retrieval {0} for type {1} with key {2}", 
                    result != null ? "successful" : "unsuccessful", typeof(TDto).Name, id);
                return result;
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(GetByIdAsync), $"String: {id}");
                throw;
            }
        }
        /// <summary>
        /// Retrieves a DTO by composite keys.
        /// </summary>
        /// <param name="key1">The first key value.</param>
        /// <param name="key1PropertyName">The name of the first key property.</param>
        /// <param name="key2">The second key value.</param>
        /// <param name="key2PropertyName">The name of the second key property.</param>
        /// <returns>The DTO if found; otherwise, null.</returns>
        /// <exception cref="ArgumentException">Thrown when any parameter is null or empty.</exception>
        public virtual async Task<TDto> GetByCompositeKeysAsync(string key1, string key1PropertyName, string key2, string key2PropertyName)
        {
            if (string.IsNullOrWhiteSpace(key1))
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_KEY1_NULL_OR_EMPTY), nameof(key1));
            if (string.IsNullOrWhiteSpace(key1PropertyName))
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_KEY1_PROPERTY_NAME_NULL_OR_EMPTY), nameof(key1PropertyName));
            if (string.IsNullOrWhiteSpace(key2))
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_KEY2_NULL_OR_EMPTY), nameof(key2));
            if (string.IsNullOrWhiteSpace(key2PropertyName))
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_KEY2_PROPERTY_NAME_NULL_OR_EMPTY), nameof(key2PropertyName));

            LogDebug("Retrieving DTO of type {0} by composite keys {1}={2}, {3}={4}", 
                typeof(TDto).Name, key1PropertyName, key1, key2PropertyName, key2);

            try
            {
                var entity = await _repository.GetByCompositeKeysAsync<string, string>(key1, key1PropertyName, key2, key2PropertyName).ConfigureAwait(false);
                var result = _mapper.Map<TDto>(entity);
                
                LogDebug("DTO retrieval {0} for type {1} with composite keys", 
                    result != null ? "successful" : "unsuccessful", typeof(TDto).Name);
                return result;
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(GetByCompositeKeysAsync), $"Keys: {key1PropertyName}={key1}, {key2PropertyName}={key2}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a collection of DTOs by composite keys.
        /// </summary>
        /// <param name="key1">The first key value.</param>
        /// <param name="key1PropertyName">The name of the first key property.</param>
        /// <param name="key2">The second key value.</param>
        /// <param name="key2PropertyName">The name of the second key property.</param>
        /// <returns>A collection of DTOs matching the composite keys.</returns>
        /// <exception cref="ArgumentException">Thrown when any parameter is null or empty.</exception>
        public virtual async Task<IEnumerable<TDto>> GetListByCompositeKeysAsync(string key1, string key1PropertyName, string key2, string key2PropertyName)
        {
            if (string.IsNullOrWhiteSpace(key1))
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_KEY1_NULL_OR_EMPTY), nameof(key1));
            if (string.IsNullOrWhiteSpace(key1PropertyName))
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_KEY1_PROPERTY_NAME_NULL_OR_EMPTY), nameof(key1PropertyName));
            if (string.IsNullOrWhiteSpace(key2))
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_KEY2_NULL_OR_EMPTY), nameof(key2));
            if (string.IsNullOrWhiteSpace(key2PropertyName))
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_KEY2_PROPERTY_NAME_NULL_OR_EMPTY), nameof(key2PropertyName));

            LogDebug("Retrieving DTOs of type {0} by composite keys {1}={2}, {3}={4}", 
                typeof(TDto).Name, key1PropertyName, key1, key2PropertyName, key2);

            try
            {
                var entities = await _repository.GetListByCompositeKeysAsync<string, string>(key1, key1PropertyName, key2, key2PropertyName).ConfigureAwait(false);
                var result = _mapper.Map<IEnumerable<TDto>>(entities);
                
                LogDebug("Successfully retrieved {0} DTOs of type {1} with composite keys", 
                    result.Count(), typeof(TDto).Name);
                return result;
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(GetListByCompositeKeysAsync), $"Keys: {key1PropertyName}={key1}, {key2PropertyName}={key2}");
                throw;
            }
        }
        /// <summary>
        /// Validates that the provided DTO parameter is not null.
        /// </summary>
        /// <param name="dto">The DTO parameter to validate.</param>
        /// <param name="parameterName">The name of the parameter for the exception message.</param>
        /// <exception cref="ArgumentNullException">Thrown when dto is null.</exception>
        protected void ValidateDtoParameter(TDto dto, string parameterName = "dto")
        {
            if (dto == null)
                throw new ArgumentNullException(parameterName);
        }

        /// <summary>
        /// Validates that a foreign key exists in the database.
        /// </summary>
        /// <typeparam name="TService">The type of the service to validate against.</typeparam>
        /// <typeparam name="TDtoType">The type of the DTO returned by the service.</typeparam>
        /// <param name="service">The service instance to use for validation.</param>
        /// <param name="id">The foreign key ID to validate.</param>
        /// <param name="entityName">The name of the entity for error messages.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the foreign key is not found.</exception>
            protected async Task ValidateForeignKeyAsync<TService, TDtoType>(TService service, Guid id, string entityName)
                where TService : class
                where TDtoType : class
            {
                // Use reflection to find and call GetByIdAsync method on the actual implementation type
                var serviceType = service.GetType();
                var method = serviceType.GetMethod("GetByIdAsync", new[] { typeof(Guid) });
                if (method is null)
                    throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_SERVICE_GETBYID_METHOD_MISSING, serviceType.Name));

                var task = (Task)method.Invoke(service, new object[] { id });
                await task;

                // Get the result from the completed task
                var resultProperty = task.GetType().GetProperty("Result");
                var result = resultProperty?.GetValue(task);

                if (result is null)
                    throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(), $"{entityName} with ID {id} not found.");
            }

        /// <summary>
        /// Validates that a nullable foreign key exists in the database if it has a value.
        /// </summary>
        /// <typeparam name="TService">The type of the service to validate against.</typeparam>
        /// <typeparam name="TDtoType">The type of the DTO returned by the service.</typeparam>
        /// <param name="service">The service instance to use for validation.</param>
        /// <param name="id">The nullable foreign key ID to validate.</param>
        /// <param name="entityName">The name of the entity for error messages.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the foreign key is not found.</exception>
        protected async Task ValidateForeignKeyAsync<TService, TDtoType>(TService service, Guid? id, string entityName)
            where TService : class
            where TDtoType : class
        {
            if (id.HasValue)
            {
                await ValidateForeignKeyAsync<TService, TDtoType>(service, id.Value, entityName);
            }
        }

        /// <summary>
        /// Validates that the DepartmentID cannot be changed during an update operation.
        /// </summary>
        /// <param name="primaryKeyId">The primary key ID of the entity being updated.</param>
        /// <param name="newDepartmentId">The new DepartmentID value from the DTO.</param>
        /// <param name="getExistingItemAsync">Function to retrieve the existing DTO by ID.</param>
        /// <exception cref="ArgumentException">Thrown when DepartmentID cannot be changed during update.</exception>
        protected async Task ValidateDepartmentIdForUpdateAsync(Guid primaryKeyId,Guid newDepartmentId, Func<Guid, Task<TDto?>> getExistingItemAsync)
        {
            if (primaryKeyId == Guid.Empty)
                return; // Skip validation for new records

            var existingItem = await getExistingItemAsync(primaryKeyId);
            if (existingItem != null)
            {
                // Use reflection to get DepartmentID property from the DTO
                var departmentIdProperty = typeof(TDto).GetProperty("DepartmentID");
                if (departmentIdProperty != null)
                {
                    var existingDepartmentId = (Guid)departmentIdProperty.GetValue(existingItem)!;
                    if (existingDepartmentId != newDepartmentId)
                        throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_DIFFERENT_DEPARTMENT_CANNOT_EDIT));
                }
            }
        }

        /// <summary>
        /// Validates authorization and determines if this is an update operation.
        /// </summary>
        /// <param name="dto">The DTO to analyze.</param>
        /// <param name="requireDepartment">Whether to validate department access.</param>
        /// <param name="requireEmployee">Whether to validate employee access.</param>
        /// <returns>A tuple containing the requested IDs and whether this is an update operation.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when required IDs are missing from token.</exception>
        private (Guid? departmentId, Guid? employeeId, bool isUpdate) ValidateAccessAndUpdateStatus(TDto dto, bool requireDepartment = false, bool requireEmployee = false)
        {
            if (requireDepartment)
            {
                if (!departmentId.HasValue)
                    throw new UnauthorizedAccessException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_DEPARTMENT_ID_MISSING_FROM_TOKEN));
            }

            if (requireEmployee)
            {
                if (!employeeId.HasValue)
                    throw new UnauthorizedAccessException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_EMPLOYEE_ID_MISSING_FROM_TOKEN));
            }

            bool isUpdate = IsUpdateOperation(dto);

            return (departmentId, employeeId, isUpdate);
        }

        /// <summary>
        /// Validates department authorization and determines if this is an update operation.
        /// </summary>
        /// <param name="dto">The DTO to analyze.</param>
        /// <returns>A tuple containing the department ID and whether this is an update operation.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when department ID is missing from token.</exception>
        protected (Guid departmentId, bool isUpdate) ValidateDepartmentAccessAndUpdateStatus(TDto dto)
        {
            var (departmentId, _, isUpdate) = ValidateAccessAndUpdateStatus(dto, requireDepartment: true);
            return (departmentId!.Value, isUpdate);
        }

        /// <summary>
        /// Validates employee authorization and determines if this is an update operation.
        /// Ensures that users can only work with records related to themselves when IEmployeeScoped.
        /// </summary>
        /// <param name="dto">The DTO to analyze.</param>
        /// <returns>A tuple containing the employee ID and whether this is an update operation.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when employee ID is missing from token.</exception>
        protected (Guid employeeId, bool isUpdate) ValidateEmployeeAccessAndUpdateStatus(TDto dto)
        {
            var (_, employeeId, isUpdate) = ValidateAccessAndUpdateStatus(dto, requireEmployee: true);
            return (employeeId!.Value, isUpdate);
        }

        /// <summary>
        /// Validates both department and employee authorization and determines if this is an update operation.
        /// For services that implement both IDepartmentScoped and IEmployeeScoped.
        /// </summary>
        /// <param name="dto">The DTO to analyze.</param>
        /// <returns>A tuple containing department ID, employee ID, and whether this is an update operation.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when department or employee ID is missing from token.</exception>
        protected (Guid departmentId, Guid employeeId, bool isUpdate) ValidateDepartmentAndEmployeeAccessAndUpdateStatus(TDto dto)
        {
            var (departmentId, employeeId, isUpdate) = ValidateAccessAndUpdateStatus(dto, requireDepartment: true, requireEmployee: true);
            return (departmentId!.Value, employeeId!.Value, isUpdate);
        }

        /// <summary>
        /// Gets the primary key value from the DTO.
        /// </summary>
        /// <param name="dto">The DTO to extract the primary key from.</param>
        /// <returns>The primary key value, or null if not found.</returns>
        private object? GetPrimaryKeyValue(TDto dto)
        {
            var primaryKeyProperty = typeof(TDto).GetProperty(_primaryKeyName);
            return primaryKeyProperty?.GetValue(dto);
        }

        /// <summary>
        /// Determines if this is an update operation based on the primary key value.
        /// </summary>
        /// <param name="dto">The DTO to analyze.</param>
        /// <returns>True if this is an update operation, false if it's a create operation.</returns>
        protected bool IsUpdateOperation(TDto dto)
        {
            var primaryKeyValue = GetPrimaryKeyValue(dto);
            return primaryKeyValue != null &&
                   ((primaryKeyValue is Guid guid && guid != Guid.Empty) ||
                    (primaryKeyValue is string str && !string.IsNullOrWhiteSpace(str)));
        }

        /// <summary>
        /// Validates that a combination of foreign key values does not already exist in the database.
        /// Supports automatic scoping based on department and employee access.
        /// </summary>
        /// <param name="dto">The DTO being validated.</param>
        /// <param name="filterExpression">Expression defining the duplicate check condition.</param>
        /// <param name="errorMessage">Error message constant for the duplicate validation exception.</param>
        /// <param name="isDepartmentScoped">Whether the validation should be scoped to the current user's department.</param>
        /// <param name="isEmployeeScoped">Whether the validation should be scoped to the current user (for personal records).</param>
        /// <param name="isDepartmentAndEmployeeScoped">Whether the validation should be scoped to both current user's department and employee.</param>
        /// <exception cref="WebApiException">Thrown when a duplicate combination is found.</exception>
        protected async Task ValidateCompositeKeyDuplicateAsync(
            TDto dto,
            Expression<Func<TEntity, bool>> filterExpression,
            string errorMessage,
            bool isDepartmentScoped = false,
            bool isEmployeeScoped = false,
            bool isDepartmentAndEmployeeScoped = false)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            if (filterExpression == null)
                throw new ArgumentNullException(nameof(filterExpression));
            if (string.IsNullOrWhiteSpace(errorMessage))
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ERROR_MESSAGE_NULL_OR_EMPTY), nameof(errorMessage));

            if (isDepartmentScoped && isEmployeeScoped)
            {
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_INVALID_SCOPE_FLAGS));
            }

            if ((isDepartmentScoped || isEmployeeScoped) && isDepartmentAndEmployeeScoped)
            {
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_SCOPE_FLAGS_CONFLICT));
            }

            try
            {
                // Get department/employee ID and update status if scoped validation is needed
                Guid? departmentId = null;
                Guid? employeeId = null;

                if (isDepartmentAndEmployeeScoped)
                {
                    var (deptId, empId, _) = ValidateDepartmentAndEmployeeAccessAndUpdateStatus(dto);
                    departmentId = deptId;
                    employeeId = empId;
                }
                else if (isDepartmentScoped)
                {
                    var (deptId, _) = ValidateDepartmentAccessAndUpdateStatus(dto);
                    departmentId = deptId;
                }
                else if (isEmployeeScoped)
                {
                    var (empId, _) = ValidateEmployeeAccessAndUpdateStatus(dto);
                    employeeId = empId;
                }

                // Get the primary key value for exclusion in updates
                var primaryKeyValue = GetPrimaryKeyValue(dto);

                // Build the final filter expression
                Expression<Func<TEntity, bool>> finalFilter = filterExpression;

                // Add department and/or employee scoping if needed
                if (isDepartmentAndEmployeeScoped)
                {
                    // Both department and employee scoping
                    if (departmentId.HasValue)
                    {
                        var parameter = filterExpression.Parameters[0];
                        var departmentProperty = Expression.Property(parameter, "DepartmentID");
                        var departmentConstant = Expression.Constant(departmentId.Value);
                        var departmentEquality = Expression.Equal(departmentProperty, departmentConstant);
                        var combined = Expression.AndAlso(filterExpression.Body, departmentEquality);
                        finalFilter = Expression.Lambda<Func<TEntity, bool>>(combined, parameter);
                    }

                    if (employeeId.HasValue)
                    {
                        var parameter = finalFilter.Parameters[0];
                        var employeeProperty = Expression.Property(parameter, "EmployeeID");
                        var employeeConstant = Expression.Constant(employeeId.Value);
                        var employeeEquality = Expression.Equal(employeeProperty, employeeConstant);
                        var combined = Expression.AndAlso(finalFilter.Body, employeeEquality);
                        finalFilter = Expression.Lambda<Func<TEntity, bool>>(combined, parameter);
                    }
                }
                else
                {
                    // Individual scoping
                    if (isDepartmentScoped && departmentId.HasValue)
                    {
                        var parameter = filterExpression.Parameters[0];
                        var departmentProperty = Expression.Property(parameter, "DepartmentID");
                        var departmentConstant = Expression.Constant(departmentId.Value);
                        var departmentEquality = Expression.Equal(departmentProperty, departmentConstant);
                        var combined = Expression.AndAlso(filterExpression.Body, departmentEquality);
                        finalFilter = Expression.Lambda<Func<TEntity, bool>>(combined, parameter);
                    }

                    if (isEmployeeScoped && employeeId.HasValue)
                    {
                        var parameter = finalFilter.Parameters[0];
                        var employeeProperty = Expression.Property(parameter, "EmployeeID");
                        var employeeConstant = Expression.Constant(employeeId.Value);
                        var employeeEquality = Expression.Equal(employeeProperty, employeeConstant);
                        var combined = Expression.AndAlso(finalFilter.Body, employeeEquality);
                        finalFilter = Expression.Lambda<Func<TEntity, bool>>(combined, parameter);
                    }
                }

                // For updates, exclude the current record
                if (primaryKeyValue != null &&
                    ((primaryKeyValue is Guid guid && guid != Guid.Empty) ||
                     (primaryKeyValue is string str && !string.IsNullOrWhiteSpace(str))))
                {
                    var parameter = finalFilter.Parameters[0];
                    var entityPrimaryKeyProperty = Expression.Property(parameter, _primaryKeyName);
                    var excludeValue = Expression.Constant(primaryKeyValue);
                    var notEqual = Expression.NotEqual(entityPrimaryKeyProperty, excludeValue);

                    // Combine the original filter with the exclusion
                    var combined = Expression.AndAlso(finalFilter.Body, notEqual);
                    finalFilter = Expression.Lambda<Func<TEntity, bool>>(combined, parameter);
                }

                var existingRecords = await _repository.GetAllAsync(finalFilter, null, null).ConfigureAwait(false);
                var hasDuplicates = existingRecords.Any();

                if (hasDuplicates)
                {
                    LogDebug("Composite key duplicate validation failed for {0}", typeof(TEntity).Name);

                    // Get the user-friendly message from the Description attribute if available
                    string userFriendlyMessage = ModelValidationErrorCodes.GetErrorMessage(errorMessage);
                    if (string.IsNullOrEmpty(userFriendlyMessage))
                        userFriendlyMessage = errorMessage; // Fallback to the error code if description not found

                    throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(), userFriendlyMessage);
                }

                LogDebug("Composite key duplicate validation passed for {0}", typeof(TEntity).Name);
            }
            catch (WebApiException)
            {
                // Re-throw WebApiException as-is
                throw;
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(ValidateCompositeKeyDuplicateAsync), $"Entity: {typeof(TEntity).Name}");
                throw new WebApiException(HttpStatusCode.InternalServerError, ((int)HttpStatusCode.InternalServerError).ToString(), "Error validating composite key duplicates");
            }
        }

        /// <summary>
        /// Validates that a property value does not already exist in the database for duplicate prevention.
        /// </summary>
        /// <typeparam name="TValue">The type of the property value to check.</typeparam>
        /// <param name="propertyName">The name of the property to check for duplicates.</param>
        /// <param name="value">The value to check for duplicates.</param>
        /// <param name="dto">The DTO being validated (used for department scoping and update detection).</param>
        /// <param name="isDepartmentScoped">Whether the validation should be scoped to the current user's department.</param>
        /// <param name="isEmployeeScoped">Whether the validation should be scoped to the current user (for personal records).</param>
        /// <param name="isDepartmentAndEmployeeScoped">Whether the validation should be scoped to both current user's department and employee.</param>
        /// <param name="additionalFilter">Optional additional filter expression to apply.</param>
        /// <param name="errorMessage">Custom error message for the duplicate validation exception.</param>
        /// <exception cref="WebApiException">Thrown when a duplicate value is found.</exception>
        protected async Task ValidateDuplicateAsync<TValue>(
            string propertyName,
            TValue value,
            TDto dto,
            bool isDepartmentScoped = false,
            bool isEmployeeScoped = false,
            bool isDepartmentAndEmployeeScoped = false,
            Expression<Func<TEntity, bool>>? additionalFilter = null,
            string errorMessage = "Duplicate value found")
        {
            if (isDepartmentScoped && isEmployeeScoped)
            {
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_INVALID_SCOPE_FLAGS));
            }

            if ((isDepartmentScoped || isEmployeeScoped) && isDepartmentAndEmployeeScoped)
            {
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_SCOPE_FLAGS_CONFLICT));
            }

            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_PROPERTY_NAME_NULL_OR_EMPTY), nameof(propertyName));
            if (value == null)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_VALUE_NULL), nameof(value));
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            try
            {
                // Get department/employee ID and update status if scoped validation is needed
                Guid? departmentId = null;
                Guid? employeeId = null;
                bool isUpdate = false;

                if (isDepartmentAndEmployeeScoped)
                {
                    var (deptId, empId, updateStatus) = ValidateDepartmentAndEmployeeAccessAndUpdateStatus(dto);
                    departmentId = deptId;
                    employeeId = empId;
                    isUpdate = updateStatus;
                }
                else if (isDepartmentScoped)
                {
                    var (deptId, updateStatus) = ValidateDepartmentAccessAndUpdateStatus(dto);
                    departmentId = deptId;
                    isUpdate = updateStatus;
                }
                else if (isEmployeeScoped)
                {
                    var (empId, updateStatus) = ValidateEmployeeAccessAndUpdateStatus(dto);
                    employeeId = empId;
                    isUpdate = updateStatus;
                }
                else
                {
                    isUpdate = IsUpdateOperation(dto);
                }

                // Build the base filter for the property check
                var parameter = Expression.Parameter(typeof(TEntity), "x");
                var property = Expression.Property(parameter, propertyName);
                var constant = Expression.Constant(value);
                var equality = Expression.Equal(property, constant);
                var filter = Expression.Lambda<Func<TEntity, bool>>(equality, parameter);

                // Combine with department and/or employee filters if scoped
                if (isDepartmentAndEmployeeScoped)
                {
                    // Both department and employee scoping
                    if (departmentId.HasValue)
                    {
                        var departmentProperty = Expression.Property(parameter, "DepartmentID");
                        var departmentConstant = Expression.Constant(departmentId.Value);
                        var departmentEquality = Expression.Equal(departmentProperty, departmentConstant);
                        var combined = Expression.AndAlso(filter.Body, departmentEquality);
                        filter = Expression.Lambda<Func<TEntity, bool>>(combined, parameter);
                    }

                    if (employeeId.HasValue)
                    {
                        var employeeProperty = Expression.Property(parameter, "EmployeeID");
                        var employeeConstant = Expression.Constant(employeeId.Value);
                        var employeeEquality = Expression.Equal(employeeProperty, employeeConstant);
                        var combined = Expression.AndAlso(filter.Body, employeeEquality);
                        filter = Expression.Lambda<Func<TEntity, bool>>(combined, parameter);
                    }
                }
                else
                {
                    // Individual scoping
                    if (isDepartmentScoped && departmentId.HasValue)
                    {
                        var departmentProperty = Expression.Property(parameter, "DepartmentID");
                        var departmentConstant = Expression.Constant(departmentId.Value);
                        var departmentEquality = Expression.Equal(departmentProperty, departmentConstant);
                        var combined = Expression.AndAlso(filter.Body, departmentEquality);
                        filter = Expression.Lambda<Func<TEntity, bool>>(combined, parameter);
                    }

                    if (isEmployeeScoped && employeeId.HasValue)
                    {
                        var employeeProperty = Expression.Property(parameter, "EmployeeID");
                        var employeeConstant = Expression.Constant(employeeId.Value);
                        var employeeEquality = Expression.Equal(employeeProperty, employeeConstant);
                        var combined = Expression.AndAlso(filter.Body, employeeEquality);
                        filter = Expression.Lambda<Func<TEntity, bool>>(combined, parameter);
                    }
                }

                // Combine with additional filter if provided
                if (additionalFilter != null)
                {
                    var combined = Expression.AndAlso(filter.Body, Expression.Invoke(additionalFilter, parameter));
                    filter = Expression.Lambda<Func<TEntity, bool>>(combined, parameter);
                }

                // Exclude the current record if updating
                if (isUpdate)
                {
                    var primaryKeyValue = GetPrimaryKeyValue(dto);

                    if (primaryKeyValue != null)
                    {
                        var entityPrimaryKeyProperty = Expression.Property(parameter, _primaryKeyName);
                        var excludeValue = Expression.Constant(primaryKeyValue);
                        var notEqual = Expression.NotEqual(entityPrimaryKeyProperty, excludeValue);
                        var combined = Expression.AndAlso(filter.Body, notEqual);
                        filter = Expression.Lambda<Func<TEntity, bool>>(combined, parameter);
                    }
                }

                var existingEntities = await GetAllAsync(filter, null, null).ConfigureAwait(false);
                var hasDuplicates = existingEntities.Any();

                if (hasDuplicates)
                {
                    LogDebug("Duplicate validation failed for {0} with value '{1}' on property '{2}'",
                        typeof(TEntity).Name, value, propertyName);

                    // Get the user-friendly message from the Description attribute if available
                    string userFriendlyMessage = ModelValidationErrorCodes.GetErrorMessage(errorMessage);
                    if (string.IsNullOrEmpty(userFriendlyMessage))
                        userFriendlyMessage = errorMessage; // Fallback to the error code if description not found

                    throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(), userFriendlyMessage);
                }

                LogDebug("Duplicate validation passed for {0} with value '{1}' on property '{2}'",
                    typeof(TEntity).Name, value, propertyName);
            }
            catch (WebApiException)
            {
                // Re-throw WebApiException as-is
                throw;
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(ValidateDuplicateAsync), $"Property: {propertyName}, Value: {value}");
                throw new WebApiException(HttpStatusCode.InternalServerError, ((int)HttpStatusCode.InternalServerError).ToString(), "Error validating duplicate values");
            }
        }

        /// <summary>
        /// Populates Created/Edited tracking fields for DTOs that inherit from BaseDto.
        /// For new records: Sets CreatedDate, CreatedUserID, CreatedBy from token.
        /// For existing records: Preserves existing Created fields and sets EditedDate, EditedUserID, EditedBy from token.
        /// </summary>
        /// <param name="dto">The DTO to populate tracking fields for.</param>
        /// <param name="primaryKeyValue">The primary key value to determine if this is a new or existing record.</param>
        private async Task PopulateAuditFieldsAsync(TDto dto, object? primaryKeyValue)
        {
            if (_tokenParser == null)
                return; // Skip if token parser is not available

            // Check if DTO inherits from BaseDto
            if (dto is not BaseDto)
                return;

            var dtoType = typeof(TDto);
            
            // Get properties using nameof to avoid hardcoded strings, but from the actual DTO type
            // This works because properties are inherited from BaseDto
            var createdDateProp = dtoType.GetProperty(nameof(BaseDto.CreatedDate));
            var createdUserIDProp = dtoType.GetProperty(nameof(BaseDto.CreatedUserID));
            var createdByProp = dtoType.GetProperty(nameof(BaseDto.CreatedBy));
            var editedDateProp = dtoType.GetProperty(nameof(BaseDto.EditedDate));
            var editedUserIDProp = dtoType.GetProperty(nameof(BaseDto.EditedUserID));
            var editedByProp = dtoType.GetProperty(nameof(BaseDto.EditedBy));

            // These should never be null if DTO inherits from BaseDto, but check for safety
            if (createdDateProp == null || createdUserIDProp == null || createdByProp == null ||
                editedDateProp == null || editedUserIDProp == null || editedByProp == null)
                return;

            var adminId = _tokenParser.GetAdminID();
            Guid? currentUserId = adminId ?? employeeId;

            if (!currentUserId.HasValue)
                return;

            var fullName = adminId.HasValue
                ? _tokenParser.GetClaimValue(ClaimNames.FullName)?.Trim()
                : null;
            if (string.IsNullOrWhiteSpace(fullName))
            {
                fullName = PersonNameFormatHelper.FormatLastNameCommaFirstName(
                    _tokenParser.GetClaimValue(ClaimNames.LastName),
                    _tokenParser.GetClaimValue(ClaimNames.FirstName)) ?? string.Empty;
            }

            // Determine if this is a new record
            bool isNewRecord = primaryKeyValue == null || 
                              (primaryKeyValue is Guid guid && guid == Guid.Empty) ||
                              (primaryKeyValue is string str && string.IsNullOrWhiteSpace(str));

            var currentTimestamp = DateTime.UtcNow;

            if (isNewRecord)
            {
                // New record: Set Created fields, clear Edited fields
                createdDateProp.SetValue(dto, currentTimestamp);
                createdUserIDProp.SetValue(dto, currentUserId.Value);
                createdByProp.SetValue(dto, fullName);
                editedDateProp.SetValue(dto, null);
                editedUserIDProp.SetValue(dto, null);
                editedByProp.SetValue(dto, null);
            }
            else
            {
                // Existing record: Preserve Created fields, set Edited fields
                // Get existing DTO to preserve Created fields
                TDto? existingDto = null;
                try
                {
                    if (primaryKeyValue is Guid guidKey && guidKey != Guid.Empty)
                    {
                        existingDto = await GetByIdAsync(guidKey);
                    }
                    else if (primaryKeyValue is string strKey && !string.IsNullOrWhiteSpace(strKey))
                    {
                        existingDto = await GetByIdAsync(strKey);
                    }
                }
                catch
                {
                    // If we can't get existing record, just proceed without preserving Created fields
                    LogDebug("Could not retrieve existing record to preserve Created fields for type {0}", typeof(TDto).Name);
                }

                if (existingDto != null)
                {
                    // Preserve Created fields from existing record (existingDto also inherits from BaseDto)
                    var existingCreatedDate = createdDateProp.GetValue(existingDto);
                    var existingCreatedUserID = createdUserIDProp.GetValue(existingDto);
                    var existingCreatedBy = createdByProp.GetValue(existingDto);

                    // Set values on the DTO (which inherits from BaseDto)
                    createdDateProp.SetValue(dto, existingCreatedDate);
                    createdUserIDProp.SetValue(dto, existingCreatedUserID);
                    createdByProp.SetValue(dto, existingCreatedBy);
                }

                // Set Edited fields
                editedDateProp.SetValue(dto, currentTimestamp);
                editedUserIDProp.SetValue(dto, currentUserId.Value);
                editedByProp.SetValue(dto, fullName);
            }
        }

        /// <summary>
        /// Sets soft-delete fields on an entity that exposes IsDeleted, DeletedAt, DeletedUserID, and DeletedBy
        /// (for example <see cref="BaseEntity"/> or invoice-related entities).
        /// Uses the same display name and user-id rules as <see cref="PopulateAuditFieldsAsync"/>.
        /// Intended for use inside <c>UpdateAsync</c> callbacks.
        /// </summary>
        /// <param name="entity">The entity instance to mark as soft-deleted.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no token parser is configured.</exception>
        protected void PopulateSoftDeleteFields<T>(T entity)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(entity);
            if (_tokenParser == null)
                throw new InvalidOperationException("Token parser is required for soft delete.");

            var userTypeString = _tokenParser.GetClaimValue(ClaimNames.UserType);
            Guid? deletedUserId;
            if (Enum.TryParse<UserType>(userTypeString, ignoreCase: true, out var userType))
            {
                deletedUserId = userType == UserType.Admin
                    ? _tokenParser.GetAdminID()
                    : employeeId;
            }
            else
            {
                deletedUserId = employeeId;
            }

            if (!deletedUserId.HasValue)
            {
                deletedUserId = _tokenParser.GetEmployeeID() ?? _tokenParser.GetAdminID();
            }

            var fullName = PersonNameFormatHelper.FormatLastNameCommaFirstName(
                _tokenParser.GetClaimValue(ClaimNames.LastName),
                _tokenParser.GetClaimValue(ClaimNames.FirstName)) ?? string.Empty;

            var deletedAt = DateTime.UtcNow;

            SetSoftDeleteProperty(entity, nameof(BaseEntity.IsDeleted), true);
            SetSoftDeleteProperty(entity, nameof(BaseEntity.DeletedAt), deletedAt);
            SetSoftDeleteProperty(entity, nameof(BaseEntity.DeletedUserID), deletedUserId);
            SetSoftDeleteProperty(entity, nameof(BaseEntity.DeletedBy), fullName);
        }

        private static void SetSoftDeleteProperty<TValue>(object entity, string propertyName, TValue value)
        {
            var property = entity.GetType().GetProperty(propertyName);
            if (property == null || !property.CanWrite)
            {
                throw new InvalidOperationException(
                    $"Type '{entity.GetType().Name}' must contain writable property '{propertyName}'.");
            }

            var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            var valueType = value?.GetType();

            if (value is null)
            {
                if (property.PropertyType.IsValueType && Nullable.GetUnderlyingType(property.PropertyType) == null)
                {
                    throw new InvalidOperationException(
                        $"Property '{propertyName}' on '{entity.GetType().Name}' cannot be set to null.");
                }

                property.SetValue(entity, null);
                return;
            }

            if (!targetType.IsAssignableFrom(valueType!))
            {
                throw new InvalidOperationException(
                    $"Property '{propertyName}' on '{entity.GetType().Name}' expects '{property.PropertyType.Name}' but got '{valueType!.Name}'.");
            }

            property.SetValue(entity, value);
        }

        /// <summary>
        /// Inserts a new DTO or updates an existing one.
        /// </summary>
        /// <param name="dto">The DTO to insert or update.</param>
        /// <param name="isSuperUser">
        /// When true, bypasses automatic audit field population (Created/Edited tracking).
        /// Defaults to false.
        /// </param>
        /// <returns>The upserted DTO.</returns>
        /// <exception cref="ArgumentNullException">Thrown when dto is null.</exception>
        public async virtual Task<TDto> UpsertAsync(TDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            LogDebug("Upserting DTO of type {0}", typeof(TDto).Name);

            try
            {
                // Get primary key value to determine if this is a new or existing record
                var primaryKeyProperty = typeof(TDto).GetProperty(_primaryKeyName);
                object? primaryKeyValue = null;
                bool isNewRecord = true;
                
                if (primaryKeyProperty != null)
                {
                    primaryKeyValue = primaryKeyProperty.GetValue(dto);
                    
                    // Determine if this is a new record (null or empty Guid)
                    isNewRecord = primaryKeyValue == null || 
                                  (primaryKeyValue is Guid guid && guid == Guid.Empty) ||
                                  (primaryKeyValue is string str && string.IsNullOrWhiteSpace(str));
                    
                    // If an ID is provided (not a new record), validate that the record exists
                    if (!isNewRecord)
                    {
                        TDto? existingDto = null;
                        try
                        {
                            if (primaryKeyValue is Guid guidKey)
                            {
                                existingDto = await GetByIdAsync(guidKey);
                            }
                            else if (primaryKeyProperty.PropertyType == typeof(Guid?) && primaryKeyValue != null)
                            {
                                var nullableGuid = (Guid?)primaryKeyValue;
                                if (nullableGuid.HasValue)
                                {
                                    existingDto = await GetByIdAsync(nullableGuid.Value);
                                }
                            }
                            else if (primaryKeyValue is string strKey && !string.IsNullOrWhiteSpace(strKey))
                            {
                                existingDto = await GetByIdAsync(strKey);
                            }
                            
                            if (existingDto == null)
                            {
                                var entityTypeName = typeof(TDto).Name.Replace("Dto", "");
                                throw new KeyNotFoundException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_ENTITY_WITH_ID_NOT_FOUND, entityTypeName, primaryKeyValue));
                            }
                        }
                        catch (KeyNotFoundException)
                        {
                            // Re-throw KeyNotFoundException as-is
                            throw;
                        }
                        catch (Exception ex)
                        {
                            // Log other exceptions but don't fail the upsert - let repository handle it
                            LogDebug("Could not validate existing record for type {0} with key {1}: {2}", 
                                typeof(TDto).Name, primaryKeyValue, ex.Message);
                        }
                    }
                    
                    // For new records, set to Guid.Empty so EF Core can use database default value
                    if (isNewRecord && (primaryKeyProperty.PropertyType == typeof(Guid) || primaryKeyProperty.PropertyType == typeof(Guid?)))
                    {
                        primaryKeyProperty.SetValue(dto, Guid.Empty);
                        primaryKeyValue = Guid.Empty;
                    }
                }

                if (!isSuperUser)
                    await PopulateAuditFieldsAsync(dto, primaryKeyValue);

                if (dto is IDepartmentScoped scopedDto && scopedDto.DepartmentID == Guid.Empty)
                {
                    if (!departmentId.HasValue)
                        throw new UnauthorizedAccessException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_DEPARTMENT_ID_MISSING_FROM_TOKEN));

                    scopedDto.DepartmentID = departmentId.Value;
                }

                if (dto is IEmployeeScoped employeeScopedDto && employeeScopedDto.EmployeeID == Guid.Empty)
                {

                    if (!employeeId.HasValue)
                        throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_EMPLOYEE_INFO_MISSING));

                    employeeScopedDto.EmployeeID = employeeId.Value;
                }

                var entity = _mapper.Map<TEntity>(dto);
                await _repository.UpsertAsync(entity, _primaryKeyName).ConfigureAwait(false);
                var result = _mapper.Map<TDto>(entity);
                
                LogDebug("Successfully upserted DTO of type {0}", typeof(TDto).Name);
                return result;
            }
                catch (Exception ex)
            {
                LogError(ex, nameof(UpsertAsync), $"DTO: {typeof(TDto).Name}");
                throw;
            }
        }

        /// <summary>
        /// Inserts multiple new DTOs or updates existing ones.
        /// </summary>
        /// <param name="dtos">The DTOs to insert or update.</param>
        /// <returns>A collection of upserted DTOs.</returns>
        /// <exception cref="ArgumentNullException">Thrown when dtos is null.</exception>
        /// <exception cref="ArgumentException">Thrown when dtos is empty.</exception>
        public async virtual Task<IEnumerable<TDto>> BulkUpsertAsync(IEnumerable<TDto> dtos, bool isSuperUser = false)
        {
            if (dtos == null)
                throw new ArgumentNullException(nameof(dtos));

            var dtosList = dtos.ToList();
            if (!dtosList.Any())
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_DTOS_COLLECTION_EMPTY), nameof(dtos));

            LogDebug("Bulk upserting {0} DTOs of type {1}", dtosList.Count, typeof(TDto).Name);

            try
            {
                // Get primary key property to determine new vs existing records
                var primaryKeyProperty = typeof(TDto).GetProperty(_primaryKeyName);
                
                // Populate audit fields for each DTO before mapping
                foreach (var dto in dtosList)
                {
                    object? primaryKeyValue = null;
                    if (primaryKeyProperty != null)
                    {
                        primaryKeyValue = primaryKeyProperty.GetValue(dto);
                        
                        // For new records, set to Guid.Empty so EF Core can use database default value
                        if (primaryKeyValue == null || 
                            (primaryKeyValue is Guid guid && guid == Guid.Empty) ||
                            (primaryKeyValue is string str && string.IsNullOrWhiteSpace(str)))
                        {
                            if (primaryKeyProperty.PropertyType == typeof(Guid) || primaryKeyProperty.PropertyType == typeof(Guid?))
                            {
                                primaryKeyProperty.SetValue(dto, Guid.Empty);
                                primaryKeyValue = Guid.Empty;
                            }
                        }
                    }
                    
                    if (!isSuperUser)
                        await PopulateAuditFieldsAsync(dto, primaryKeyValue);
                    
                    if (dto is IDepartmentScoped scopedDto)
                    {
                        if (departmentId.HasValue)
                        {
                            scopedDto.DepartmentID = departmentId.Value;
                        }
                    }
                    
                    if (dto is IEmployeeScoped employeeScopedDto && employeeScopedDto.EmployeeID == Guid.Empty)
                    {
                        var tokenEmployeeId = _tokenParser?.GetEmployeeID();
                        if (tokenEmployeeId.HasValue)
                        {
                            employeeScopedDto.EmployeeID = tokenEmployeeId.Value;
                        }
                    }
                }
                
                var entities = _mapper.Map<IEnumerable<TEntity>>(dtosList);
                await _repository.BulkUpsertAsync(entities, _primaryKeyName).ConfigureAwait(false);
                var result = _mapper.Map<IEnumerable<TDto>>(entities);
                
                LogDebug("Successfully bulk upserted {0} DTOs of type {1}", 
                    result.Count(), typeof(TDto).Name);
                return result;
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(BulkUpsertAsync), $"DTO: {typeof(TDto).Name}, Count: {dtosList.Count}");
                throw;
            }
        }
        /// <summary>
        /// Updates the primary key value of an entity.
        /// </summary>
        /// <param name="oldKey">The current primary key value.</param>
        /// <param name="newKey">The new primary key value.</param>
        /// <param name="updateProperty">The property to update (optional).</param>
        /// <returns>True if the primary key was updated; otherwise, false.</returns>
        /// <exception cref="ArgumentException">Thrown when oldKey or newKey is null or empty.</exception>
        public async virtual Task<bool> UpdatePrimaryKeyAsync(string oldKey, string newKey, string updateProperty = null)
        {
            if (string.IsNullOrWhiteSpace(oldKey))
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_OLD_KEY_NULL_OR_EMPTY), nameof(oldKey));
            if (string.IsNullOrWhiteSpace(newKey))
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_NEW_KEY_NULL_OR_EMPTY), nameof(newKey));

            LogDebug("Updating primary key for entity type {0} from {1} to {2}", 
                typeof(TEntity).Name, oldKey, newKey);

            try
            {
                var result = await _repository.UpdatePrimaryKeyAsync(oldKey, newKey, updateProperty).ConfigureAwait(false);
                LogDebug("Primary key update {0} for entity type {1}", 
                    result ? "successful" : "unsuccessful", typeof(TEntity).Name);
                return result;
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(UpdatePrimaryKeyAsync), $"OldKey: {oldKey}, NewKey: {newKey}");
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
        public virtual async Task<int> UpdateAsync(Expression<Func<TEntity, bool>> filter, Action<TEntity> updateAction)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));
            if (updateAction == null)
                throw new ArgumentNullException(nameof(updateAction));

            LogDebug("Updating entities of type {0} with custom filter", typeof(TEntity).Name);

            try
            {
                var result = await _repository.UpdateAsync(filter, updateAction).ConfigureAwait(false);
                LogDebug("Successfully updated {0} entities of type {1}", result, typeof(TEntity).Name);
                return result;
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(UpdateAsync), $"Entity: {typeof(TEntity).Name}");
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

            LogDebug("Deleting entity of type {0} with key {1}", typeof(TEntity).Name, id);

            try
            {
                var result = await _repository.DeleteAsync(id).ConfigureAwait(false);
                if (!result)
                {
                    throw new WebApiException(
                        HttpStatusCode.BadRequest,
                        ((int)HttpStatusCode.BadRequest).ToString(),
                        $"{typeof(TEntity).Name} with ID {id} not found.");
                }
                LogDebug("Entity deletion successful for type {0} with key {1}", typeof(TEntity).Name, id);
                return true;
            }
            catch (WebApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                var isForeignKeyViolation = ex.GetType().FullName?.Contains(ModelValidationErrorCodes.DbUpdateExceptionTypeName) == true
                    || message.Contains(ModelValidationErrorCodes.ForeignKeyMessageSubstring, StringComparison.OrdinalIgnoreCase)
                    || message.Contains(ModelValidationErrorCodes.ReferenceConstraintMessageSubstring, StringComparison.OrdinalIgnoreCase);
                if (isForeignKeyViolation)
                {
                    throw new WebApiException(
                        HttpStatusCode.BadRequest,
                        ((int)HttpStatusCode.BadRequest).ToString(),
                        ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.DELETE_DEPENDENT_RECORDS_FIRST));
                }
                LogError(ex, nameof(DeleteAsync), $"Entity: {typeof(TEntity).Name}, Key: {id}");
                throw;
            }
        }

        /// <summary>
        /// Deletes multiple entities by their key values.
        /// </summary>
        /// <typeparam name="TKey">The type of the key values.</typeparam>
        /// <param name="ids">The key values of entities to delete.</param>
        /// <returns>True if any entities were deleted; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when ids is null.</exception>
        /// <exception cref="ArgumentException">Thrown when ids is empty.</exception>
        public async Task<bool> BulkDeleteAsync<TKey>(List<TKey> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));
            if (!ids.Any())
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_IDS_COLLECTION_EMPTY), nameof(ids));

            LogDebug("Bulk deleting entities of type {0} with {1} keys", typeof(TEntity).Name, ids.Count);

            try
            {
                var result = await _repository.BulkDeleteAsync(ids, _primaryKeyName).ConfigureAwait(false);
                LogDebug("Bulk deletion {0} for {1} entities of type {2}", 
                    result ? "successful" : "unsuccessful", ids.Count, typeof(TEntity).Name);
                return result;
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(BulkDeleteAsync), $"Entity: {typeof(TEntity).Name}, Count: {ids.Count}");
                throw;
            }
        }

        /// <summary>
        /// Deletes entities by a specific property value.
        /// </summary>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="propertyName">The name of the property to filter by.</param>
        /// <param name="value">The value to match.</param>
        /// <returns>True if any entities were deleted; otherwise, false.</returns>
        /// <exception cref="ArgumentException">Thrown when propertyName is null or empty.</exception>
        public async Task<bool> DeleteByPropertyAsync<TValue>(string propertyName, TValue value)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_PROPERTY_NAME_NULL_OR_EMPTY), nameof(propertyName));

            LogDebug("Deleting entities of type {0} by property {1}={2}", 
                typeof(TEntity).Name, propertyName, value);

            try
            {
                var result = await _repository.DeleteByPropertyAsync(propertyName, value).ConfigureAwait(false);
                LogDebug("Property-based deletion {0} for entities of type {1}", 
                    result ? "successful" : "unsuccessful", typeof(TEntity).Name);
            return result;
        }
            catch (Exception ex)
            {
                LogError(ex, nameof(DeleteByPropertyAsync), $"Entity: {typeof(TEntity).Name}, Property: {propertyName}");
                throw;
            }
        }
        /// <summary>
        /// Adds multiple DTOs to the repository.
        /// </summary>
        /// <param name="clientDtos">The DTOs to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when clientDtos is null.</exception>
        /// <exception cref="ArgumentException">Thrown when clientDtos is empty.</exception>
        public async Task AddRangeAsync(IEnumerable<TDto> clientDtos)
        {
            if (clientDtos == null)
                throw new ArgumentNullException(nameof(clientDtos));

            var dtosList = clientDtos.ToList();
            if (!dtosList.Any())
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_CLIENT_DTOS_COLLECTION_EMPTY), nameof(clientDtos));

            LogDebug("Adding {0} DTOs of type {1}", dtosList.Count, typeof(TDto).Name);

            try
            {
                var entities = _mapper.Map<IEnumerable<TEntity>>(dtosList);
                await _repository.AddRangeAsync(entities).ConfigureAwait(false);
                LogDebug("Successfully added {0} DTOs of type {1}", dtosList.Count, typeof(TDto).Name);
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(AddRangeAsync), $"DTO: {typeof(TDto).Name}, Count: {dtosList.Count}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves DTOs by multiple composite key combinations.
        /// </summary>
        /// <param name="compositeKeys">A collection of dictionaries containing key-value pairs for composite keys.</param>
        /// <returns>A collection of DTOs matching any of the composite key combinations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when compositeKeys is null.</exception>
        /// <exception cref="ArgumentException">Thrown when compositeKeys is empty.</exception>
        public async Task<IEnumerable<TDto>> GetListByCompositeKeyArrayAsync(IEnumerable<Dictionary<string, string>> compositeKeys)
        {
            if (compositeKeys == null)
                throw new ArgumentNullException(nameof(compositeKeys));

            var keysList = compositeKeys.ToList();
            if (!keysList.Any())
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_COMPOSITE_KEYS_COLLECTION_EMPTY), nameof(compositeKeys));

            LogDebug("Retrieving DTOs of type {0} by {1} composite key combinations", 
                typeof(TDto).Name, keysList.Count);

            try
            {
                var entities = await _repository.GetListByCompositeKeyArrayAsync(compositeKeys).ConfigureAwait(false);
                var result = _mapper.Map<IEnumerable<TDto>>(entities);
                
                LogDebug("Successfully retrieved {0} DTOs of type {1} by composite keys", 
                    result.Count(), typeof(TDto).Name);
                return result;
            }
            catch (Exception ex)
            {
                LogError(ex, nameof(GetListByCompositeKeyArrayAsync), $"DTO: {typeof(TDto).Name}, Count: {keysList.Count}");
                throw;
            }
        }
        // Using GetCurrentDepartmentDateByTimeZoneId(department.TimeZoneId)
        // because the user's token is not available during Department Admin creation,
        // so the timezone cannot be retrieved from the token at that stage.
        protected DateOnly GetCurrentDepartmentDateByTimeZoneId(string timeZoneId)
        {
            return DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId)));
        }
        protected TimeZoneInfo GetDepartmentTimeZone()
        {
            var timeZoneId = _tokenParser.GetTimeZoneId();
            if (string.IsNullOrWhiteSpace(timeZoneId))
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_TIMEZONE_ID_REQUIRED), nameof(timeZoneId));

            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        protected DateTime GetCurrentDepartmentDateTime()
        {
            var timeZone = GetDepartmentTimeZone();
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
        }
        protected TimeOnly GetCurrentDepartmentTime()
        {
            return TimeOnly.FromDateTime(GetCurrentDepartmentDateTime());
        }

        /// <summary>
        /// Returns <c>true</c> when <paramref name="shiftStartDateTime"/> is more than 72 hours after the
        /// department-local "now" (same instant as <see cref="GetCurrentDepartmentTime"/> / <see cref="GetCurrentDepartmentDate"/>).
        /// </summary>
        protected bool IsTimeGreaterThan72Hours(DateTime shiftStartDateTime)
        {
            var nowInDepartmentTimeZone = GetCurrentDepartmentDateTime();
            return (shiftStartDateTime - nowInDepartmentTimeZone).TotalHours > 72;
        }

        protected DateOnly GetCurrentDepartmentDate()
        {
            return DateOnly.FromDateTime(GetCurrentDepartmentDateTime());
        }
        protected DateOnly NormalizeDateToDepartmentToday(DateOnly requestedDate)
        {
            // When the client used UTC calendar "today", align to the department's local calendar date.
            var departmentToday = GetCurrentDepartmentDate();
            return requestedDate == departmentToday ? departmentToday : requestedDate;
        }
        protected DateTime ResolveDepartmentDateTime(DateOnly date)
        {
            var now = GetCurrentDepartmentDateTime();
            var today = DateOnly.FromDateTime(now);

            return date == today
                ? date.ToDateTime(TimeOnly.FromDateTime(now))
                : date.ToDateTime(TimeOnly.MinValue);
        }
        protected (DateOnly NormalizedDate, DateTime EffectiveDateTime)GetDateAsPerDepartmentTimeZone(DateOnly requestedDate)
        {
            // Get department timezone via token / context
            var normalizedDate = NormalizeDateToDepartmentToday(requestedDate);

            var effectiveDateTime = ResolveDepartmentDateTime(normalizedDate);

            return (normalizedDate, effectiveDateTime);
        }
        private static readonly ConcurrentDictionary<Type, PropertyInfo?> _departmentPropCache = new();

        private Expression<Func<TEntity, bool>> AppendDepartmentFilter(
            Expression<Func<TEntity, bool>>? existingFilter)
        {
            // No department in token → no filtering
            if (!departmentId.HasValue)
                return existingFilter ?? (x => true);

            var property = _departmentPropCache.GetOrAdd(
                typeof(TEntity),
                t => t.GetProperty("DepartmentID"));

            if (property == null)
                return existingFilter ?? (x => true);

            var parameter = existingFilter?.Parameters[0]
                ?? Expression.Parameter(typeof(TEntity), "x");

            var left = Expression.Property(parameter, property);

            Expression right = property.PropertyType == typeof(Guid?)
                ? Expression.Constant(departmentId.Value, typeof(Guid?))
                : Expression.Constant(departmentId.Value);

            var departmentBody = Expression.Equal(left, right);

            if (existingFilter == null)
                return Expression.Lambda<Func<TEntity, bool>>(departmentBody, parameter);

            var replacedBody = new ReplaceParameterVisitor(
                existingFilter.Parameters[0],
                parameter).Visit(existingFilter.Body);

            var combined = Expression.AndAlso(replacedBody!, departmentBody);

            return Expression.Lambda<Func<TEntity, bool>>(combined, parameter);
        }
    }
}
