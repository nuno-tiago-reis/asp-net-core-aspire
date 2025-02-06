namespace Memento.Aspire.Shared.Persistence;

using Memento.Aspire.Shared.Pagination;

/// <summary>
/// Defines the generic interface for an entity repository.
/// Provides methods to interact with the entities (CRUD and more).
/// </summary>
///
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TEntityFilter">The entity filter type.</typeparam>
/// <typeparam name="TEntityOrderBy">The entity order by type.</typeparam>
/// <typeparam name="TEntityOrderDirection">The entity order direction type.</typeparam>
public interface IEntityRepository<TEntity, TEntityFilter, TEntityOrderBy, TEntityOrderDirection>
	where TEntity : class, IEntity
	where TEntityFilter : class, IEntityFilter<TEntityOrderBy, TEntityOrderDirection>
	where TEntityOrderBy : struct, Enum
	where TEntityOrderDirection : struct, Enum
{
	#region [Methods]
	/// <summary>
	/// Creates the specified entity.
	/// </summary>
	///
	/// <param name="entity">The entity.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);

	/// <summary>
	/// Updates the specified entity.
	/// </summary>
	///
	/// <param name="entity">The entity.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

	/// <summary>
	/// Deletes the entity matching the given id.
	/// </summary>
	///
	/// <param name="entityId">The entity identifier.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	Task DeleteAsync(Guid entityId, CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets the entity matching the given id.
	/// </summary>
	///
	/// <param name="entityId">The entity identifier.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	Task<TEntity> GetAsync(Guid entityId, CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets all the entities matching the given filter.
	/// </summary>
	///
	/// <param name="entityFilter">The entity filter.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	Task<IPage<TEntity>> GetAllAsync(TEntityFilter entityFilter, CancellationToken cancellationToken = default);

	/// <summary>
	/// Checks if a entity matching the given id exists.
	/// </summary>
	///
	/// <param name="entityId">The entity identifier.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	Task<bool> ExistsAsync(Guid entityId, CancellationToken cancellationToken = default);
	#endregion
}
