namespace Memento.Aspire.Domain.Service.Contracts.Genre;

using Memento.Aspire.Shared.Contracts;
using Memento.Aspire.Shared.Resources;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

/// <summary>
/// Implements the 'Genre' summary contract.
/// </summary>
public sealed record GenreSummaryContract : EntityContract
{
	#region [Properties]
	/// <summary>
	/// The genre's name.
	/// </summary>
	[Display(Name = nameof(SharedResources.GENRE_NAME), ResourceType = typeof(SharedResources))]
	[JsonPropertyOrder(0)]
	public required string Name { get; set; }
	#endregion
}
