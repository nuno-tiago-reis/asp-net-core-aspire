namespace Memento.Aspire.Core.Persistence;

/// <summary>
/// Defines the generic interface for an entity context.
/// Provides methods to automatically maintain traceability during create and update operations.
/// </summary>
public interface IEntityContext
{
	#region [Methods]
	/// <summary>
	/// Updates the changes made to the context and automatically updates the models timestamps.
	/// </summary>
	int SaveChanges();

	/// <summary>
	/// Updates the changes made to the context and automatically updates the models timestamps.
	/// </summary>
	///
	/// <param name="acceptAllChangesOnSuccess">Whether to accept all changes on success.</param>
	int SaveChanges(bool acceptAllChangesOnSuccess);

	/// <summary>
	/// Updates the changes made to the context and automatically updates the models timestamps.
	/// </summary>
	///
	/// <param name="cancellationToken">The cancellation token.</param>
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Updates the changes made to the context and automatically updates the models timestamps.
	/// </summary>
	///
	/// <param name="acceptAllChangesOnSuccess">Whether to accept all changes on success.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);
	#endregion
}
