namespace Memento.Aspire.Domain.Service.Contracts.Author;

using Memento.Aspire.Shared.Contracts;
using Memento.Aspire.Shared.Resources;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

/// <summary>
/// Implements the 'Author' summary contract.
/// </summary>
public sealed record AuthorSummaryContract : EntityContract
{
	#region [Properties]
	/// <summary>
	/// The author's name.
	/// </summary>
	[Display(Name = nameof(SharedResources.AUTHOR_NAME), ResourceType = typeof(SharedResources))]
	[JsonPropertyOrder(0)]
	public required string Name { get; init; }

	/// <summary>
	/// The author's birth date.
	/// </summary>
	[Display(Name = nameof(SharedResources.AUTHOR_BIRTHDATE), ResourceType = typeof(SharedResources))]
	[JsonPropertyOrder(1)]
	public required DateOnly BirthDate { get; init; }
	#endregion
}
