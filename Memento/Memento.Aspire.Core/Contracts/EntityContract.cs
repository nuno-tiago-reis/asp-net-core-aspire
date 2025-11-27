namespace Memento.Aspire.Core.Contracts;

using Memento.Aspire.Core.Resources;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

/// <summary>
/// Implements a generic entity contract.
/// Provides properties to maintain traceability during create and update operations.
/// </summary>
public abstract record EntityContract
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the identifier.
	/// </summary>
	[Display(Name = nameof(SharedResources.ENTITY_ID), ResourceType = typeof(SharedResources))]
	[JsonPropertyOrder(int.MinValue)]
	public required Guid Id { get; init; }
	#endregion
}
