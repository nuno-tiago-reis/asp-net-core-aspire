namespace Memento.Aspire.Shared.Persistence;

using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Implements the generic interface for an entity context.
/// Provides methods to automatically maintain traceability during create and update operations.
/// </summary>
public abstract class EntityContext : DbContext, IEntityContext
{
	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="EntityContext"/> class.
	/// </summary>
	///
	/// <param name="options">The options.</param>
	protected EntityContext(DbContextOptions options) : base(options)
	{
		// Intentionally Empty.
	}
	#endregion

	#region [Methods]
	/// <summary>
	/// Updates the changes made to the context and automatically updates the models timestamps.
	/// </summary>
	public override int SaveChanges()
	{
		this.UpdateModelTimestamps();

		return base.SaveChanges();
	}

	/// <summary>
	/// Updates the changes made to the context and automatically updates the models timestamps.
	/// </summary>
	///
	/// <param name="acceptAllChangesOnSuccess">Whether to accept all changes on success.</param>
	public override int SaveChanges(bool acceptAllChangesOnSuccess)
	{
		this.UpdateModelTimestamps();

		return base.SaveChanges(acceptAllChangesOnSuccess);
	}

	/// <summary>
	/// Updates the changes made to the context and automatically updates the models timestamps.
	/// </summary>
	///
	/// <param name="cancellationToken">The cancellation token.</param>
	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		this.UpdateModelTimestamps();

		return base.SaveChangesAsync(cancellationToken);
	}

	/// <summary>
	/// Updates the changes made to the context and automatically updates the models timestamps.
	/// </summary>
	///
	/// <param name="acceptAllChangesOnSuccess">Whether to accept all changes on success.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
	{
		this.UpdateModelTimestamps();

		return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
	}

	/// <summary>
	/// Updates the entries in the change tracker that were either created or updated.
	/// - If an entry was created, then the 'CreatedAt' field is automatically populated.
	/// - If an entry was updated, then the 'UpdatedAt' field is automatically populated.
	/// </summary>
	private void UpdateModelTimestamps()
	{
		// Find entries that were created
		var createdEntries = this.ChangeTracker.Entries().Where((entry) => entry.State == EntityState.Added);

		// Update their 'CreatedAt' fields if they implement 'IModel'
		foreach (var createdEntry in createdEntries)
		{
			if (createdEntry.Entity is IEntity model)
			{
				model.CreatedAt = DateTimeOffset.UtcNow;
			}
		}

		// Find entries that were created
		var modifiedEntries = this.ChangeTracker.Entries().Where((entry) => entry.State == EntityState.Modified);

		// Update their 'UpdatedAt' fields if they implement 'IModel'
		foreach (var modifiedEntry in modifiedEntries)
		{
			if (modifiedEntry.Entity is IEntity model)
			{
				model.UpdatedAt = DateTimeOffset.UtcNow;
			}
		}
	}
	#endregion
}
