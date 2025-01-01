namespace Memento.Aspire.Domain.Service.Contracts.Genre;

using Memento.Aspire.Shared.Resources;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Implements the 'Genre' form contract.
/// </summary>
public sealed record GenreFormContract
{
	#region [Properties]
	/// <summary>
	/// The genre's name.
	/// </summary>
	[Display(Name = nameof(SharedResources.GENRE_NAME), ResourceType = typeof(SharedResources))]
	public required string? Name { get; set; }
	#endregion
}
