namespace Memento.Aspire.Shared.Persistence;

/// <summary>
/// Defines the generic interface for an entity.
/// Provides properties to maintain traceability during create and update operations.
/// </summary>
public interface IEntity
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the identifier.
	/// </summary>
	Guid Id { get; set; }

	/// <summary>
	/// Gets or sets the created by user identifier.
	/// </summary>
	Guid CreatedBy { get; set; }

	/// <summary>
	/// Gets or sets the created at timestamp.
	/// </summary>
	DateTimeOffset CreatedAt { get; set; }

	/// <summary>
	/// Gets or sets the updated by user identifier.
	/// </summary>
	Guid? UpdatedBy { get; set; }

	/// <summary>
	/// Gets or sets the updated at timestamp.
	/// </summary>
	DateTimeOffset? UpdatedAt { get; set; }
	#endregion
}
