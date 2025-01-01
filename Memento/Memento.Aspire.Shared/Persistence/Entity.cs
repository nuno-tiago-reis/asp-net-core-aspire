namespace Memento.Aspire.Shared.Persistence;

using Memento.Aspire.Shared.Resources;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Implements the generic interface for an entity.
/// Provides properties to maintain traceability during create and update operations.
/// </summary>
public abstract record Entity : IEntity
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the identifier.
	/// </summary>
	[Display(Name = nameof(SharedResources.ENTITY_ID), ResourceType = typeof(SharedResources))]
	public required Guid Id { get; set; }

	/// <summary>
	/// Gets or sets the created by user identifier.
	/// </summary>
	[Display(Name = nameof(SharedResources.ENTITY_CREATEDBY), ResourceType = typeof(SharedResources))]
	public required Guid CreatedBy { get; set; }

	/// <summary>
	/// Gets or sets the created at timestamp.
	/// </summary>
	[Display(Name = nameof(SharedResources.ENTITY_CREATEDAT), ResourceType = typeof(SharedResources))]
	public required DateTimeOffset CreatedAt { get; set; }

	/// <summary>
	/// Gets or sets the updated by user identifier.
	/// </summary>
	[Display(Name = nameof(SharedResources.ENTITY_UPDATEDBY), ResourceType = typeof(SharedResources))]
	public Guid? UpdatedBy { get; set; }

	/// <summary>
	/// Gets or sets the updated at timestamp.
	/// </summary>
	[Display(Name = nameof(SharedResources.ENTITY_UPDATEDAT), ResourceType = typeof(SharedResources))]
	public DateTimeOffset? UpdatedAt { get; set; }
	#endregion
}
