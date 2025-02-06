namespace Memento.Aspire.Shared.Persistence;

using Memento.Aspire.Shared.Exceptions;
using Memento.Aspire.Shared.Localization;
using Memento.Aspire.Shared.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

/// <summary>
/// Implements the generic interface for an entity repository.
/// Provides methods to interact with the entities (CRUD and more).
/// </summary>
///
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TEntityFilter">The entity filter type.</typeparam>
/// <typeparam name="TEntityOrderBy">The entity order by type.</typeparam>
/// <typeparam name="TEntityOrderDirection">The entity order direction type.</typeparam>
public abstract class EntityRepository<TEntity, TEntityFilter, TEntityOrderBy, TEntityOrderDirection> : IEntityRepository<TEntity, TEntityFilter, TEntityOrderBy, TEntityOrderDirection>
	where TEntity : class, IEntity
	where TEntityFilter : class, IEntityFilter<TEntityOrderBy, TEntityOrderDirection>
	where TEntityOrderBy : struct, Enum
	where TEntityOrderDirection : struct, Enum
{
	#region [Properties]
	/// <summary>
	/// The context.
	/// </summary>
	protected DbContext Context { get; init; }

	/// <summary>
	/// The entities.
	/// </summary>
	protected DbSet<TEntity> Entities { get; init; }

	/// <summary>
	/// The localizer service.
	/// </summary>
	protected ILocalizer Localizer { get; init; }

	/// <summary>
	/// The logger.
	/// </summary>
	protected ILogger Logger { get; init; }
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="EntityRepository{TEntity, TEntityFilter, TEntityFilterOrderBy, TEntityFilterOrderDirection}"/> class.
	/// </summary>
	///
	/// <param name="context">The context.</param>
	/// <param name="localizer">The localizer.</param>
	/// <param name="logger">The logger.</param>
	protected EntityRepository
	(
		DbContext context,
		ILocalizer localizer,
		ILogger<EntityRepository<TEntity, TEntityFilter, TEntityOrderBy, TEntityOrderDirection>> logger
	)
	{
		this.Context = context;
		this.Entities = context.Set<TEntity>();
		this.Localizer = localizer;
		this.Logger = logger;
	}
	#endregion

	#region [Methods] Interface
	/// <summary>
	/// Creates the specified entity.
	/// </summary>
	///
	/// <param name="entity">The entity.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
	{
		// Normalize the entity
		this.NormalizeEntity(entity);
		// Validate the entity
		this.ValidateEntity(entity);

		// Create the entity
		await this.Entities.AddAsync(entity, cancellationToken);
		// Save the changes
		await this.Context.SaveChangesAsync(cancellationToken);

		// Detach the entity before returning it
		this.Context.Entry(entity).State = EntityState.Detached;

		// Ensure the navigation properties are included
		entity = await this.GetAsync(entity.Id, cancellationToken);

		return entity;
	}

	/// <summary>
	/// Updates the specified entity.
	/// </summary>
	///
	/// <param name="entity">The entity.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
	{
		// Check if the entity exists
		var databaseEntity = await this.TryGetEntityAsync(entity.Id, cancellationToken);

		// Normalize the entity
		this.NormalizeEntity(entity);
		// Validate the entity
		this.ValidateEntity(entity);

		// Update the entity
		this.UpdateEntity(entity, databaseEntity);
		// Save the changes
		await this.Context.SaveChangesAsync(cancellationToken);

		// Detach the entity before returning it
		this.Context.Entry(entity).State = EntityState.Detached;

		// Ensure the navigation properties are included
		entity = await this.GetAsync(entity.Id, cancellationToken);

		return entity;
	}

	/// <summary>
	/// Deletes the entity matching the given id.
	/// </summary>
	///
	/// <param name="entityId">The entity identifier.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public virtual async Task DeleteAsync(Guid entityId, CancellationToken cancellationToken = default)
	{
		// Check if the entity exists
		var entity = await this.TryGetEntityAsync(entityId, cancellationToken);

		// Delete the entity
		this.Entities.Remove(entity);
		// Save the changes
		await this.Context.SaveChangesAsync(cancellationToken);
	}

	/// <summary>
	/// Gets the entity matching the given id.
	/// </summary>
	///
	/// <param name="entityId">The entity identifier.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public virtual async Task<TEntity> GetAsync(Guid entityId, CancellationToken cancellationToken = default)
	{
		// Check if the entity exists
		var _ = await this.TryGetEntityAsync(entityId, cancellationToken);

		// Get the entity
		var entity = await this.GetEntityDetailQueryable().FirstAsync((entity) => entity.Id == entityId, cancellationToken);

		// Detach the entity before returning it
		this.Context.Entry(entity).State = EntityState.Detached;

		return entity;
	}

	/// <summary>
	/// Gets all the entities matching the given filter.
	/// </summary>
	///
	/// <param name="entityFilter">The entity filter.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public virtual async Task<IPage<TEntity>> GetAllAsync(TEntityFilter entityFilter, CancellationToken cancellationToken = default)
	{
		// Get the queryable
		var entityQuery = this.GetEntitySummaryQueryable();
		var entityCountQuery = this.GetEntityCountQueryable();

		// Filter the queryables
		entityQuery = this.FilterQueryable(entityQuery, entityFilter).AsNoTracking();
		entityCountQuery = this.FilterQueryable(entityCountQuery, entityFilter).AsNoTracking();

		// Create the entity page
		var entities = await Page<TEntity>.CreateAsync
		(
			// entities
			entityQuery,
			entityCountQuery,
			// entity pagination
			entityFilter.PageNumber,
			entityFilter.PageSize,
			entityFilter.OrderBy.ToString(),
			entityFilter.OrderDirection.ToString()
		);

		// Detach the entities before returning them
		foreach (var entity in entities)
		{
			this.Context.Entry(entity).State = EntityState.Detached;
		}

		return entities;
	}

	/// <summary>
	/// Checks if a entity matching the given id exists.
	/// </summary>
	///
	/// <param name="entityId">The entity identifier.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public virtual async Task<bool> ExistsAsync(Guid entityId, CancellationToken cancellationToken = default)
	{
		return await this.Entities.AnyAsync((entity) => entity.Id == entityId, cancellationToken);
	}
	#endregion

	#region [Methods] Changes
	/// <summary>
	/// Normalizes the entity.
	/// </summary>
	///
	/// <param name="entity">The entity.</param>
	protected abstract void NormalizeEntity(TEntity entity);

	/// <summary>
	/// Validates the entity.
	/// </summary>
	///
	/// <param name="entity">The entity.</param>
	protected abstract void ValidateEntity(TEntity entity);

	/// <summary>
	/// Updates the entity.
	/// </summary>
	///
	/// <param name="sourceEntity">The source entity.</param>
	/// <param name="targetEntity">The target entity.</param>
	protected abstract void UpdateEntity(TEntity sourceEntity, TEntity targetEntity);
	#endregion

	#region [Methods] Queryables
	/// <summary>
	/// Gets an entity summary queryable to be used in entity queries.
	/// This queryable should only include the base entity.
	/// </summary>
	protected abstract IQueryable<TEntity> GetEntitySummaryQueryable();

	/// <summary>
	/// Gets an entity detail queryable to be used in entity queries.
	/// This queryable should include the base entity as well as any relations.
	/// </summary>
	protected abstract IQueryable<TEntity> GetEntityDetailQueryable();

	/// <summary>
	/// Gets an entity acount queryable to be used in entity count queries.
	/// This queryable should only include the base entity.
	/// </summary>
	protected abstract IQueryable<TEntity> GetEntityCountQueryable();

	/// <summary>
	/// Filters the entity queryable.
	/// </summary>
	///
	/// <param name="entityQueryable">The entity queryable.</param>
	/// <param name="entityFilter">The entity filter.</param>
	protected abstract IQueryable<TEntity> FilterQueryable(IQueryable<TEntity> entityQueryable, TEntityFilter entityFilter);
	#endregion

	#region [Methods] Messages
	/// <summary>
	/// Returns a message indicating that the given entity does not exist.
	/// </summary>
	protected abstract string GetEntityDoesNotExistMessage();

	/// <summary>
	/// Returns a message indicating that the given entities field is duplicated.
	/// </summary>
	///
	/// <param name="expression">The expression.</param>
	protected abstract string GetEntityHasDuplicateFieldMessage<TProperty>(Expression<Func<TEntity, TProperty>> expression);

	/// <summary>
	/// Returns a message indicating that the given entities field combination is duplicated.
	/// </summary>
	///
	/// <param name="expression">The expression.</param>
	protected abstract string GetEntityHasDuplicateFieldCombinationMessage(params Expression<Func<TEntity, object>>[] expressions);

	/// <summary>
	/// Returns a message indicating that the given entities field is invalid.
	/// </summary>
	///
	/// <param name="expression">The expression.</param>
	protected abstract string GetEntityHasInvalidFieldMessage<TProperty>(Expression<Func<TEntity, TProperty>> expression);
	#endregion

	#region [Methods] Helpers
	/// <summary>
	/// Tries to get an entity matching the given id if it exists.
	/// </summary>
	///
	/// <param name="entityId">The entity identifier.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	protected async Task<TEntity> TryGetEntityAsync(Guid? entityId, CancellationToken cancellationToken = default)
	{
		var entity = await this.Entities.FirstOrDefaultAsync((entity) => entity.Id == entityId, cancellationToken);

		if (entity is null)
		{
			throw new StandardException(this.GetEntityDoesNotExistMessage(), StandardExceptionType.NotFound);
		}

		return entity;
	}
	#endregion
}
