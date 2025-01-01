namespace Memento.Aspire.Domain.Service.Contracts.Genre;

using Memento.Aspire.Domain.Service.Contracts.Book;
using Memento.Aspire.Shared.Contracts;
using Memento.Aspire.Shared.Resources;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

/// <summary>
/// Implements the 'Genre' detail contract.
/// </summary>
public sealed record GenreDetailContract : EntityContract
{
	#region [Properties]
	/// <summary>
	/// The genre's name.
	/// </summary>
	[Display(Name = nameof(SharedResources.GENRE_NAME), ResourceType = typeof(SharedResources))]
	[JsonPropertyOrder(0)]
	public required string Name { get; set; }

	/// <summary>
	/// The genre's books.
	/// </summary>
	[Display(Name = nameof(SharedResources.GENRE_BOOKS), ResourceType = typeof(SharedResources))]
	[JsonPropertyOrder(1)]
	public required List<BookSummaryContract> Books { get; set; }
	#endregion
}
