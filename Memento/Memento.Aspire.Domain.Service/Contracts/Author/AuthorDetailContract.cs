namespace Memento.Aspire.Domain.Service.Contracts.Author;

using Memento.Aspire.Domain.Service.Contracts.Book;
using Memento.Aspire.Shared.Contracts;
using Memento.Aspire.Shared.Resources;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

/// <summary>
/// Implements the 'Author' detail contract.
/// </summary>
public sealed record AuthorDetailContract : EntityContract
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

	/// <summary>
	/// The author's books.
	/// </summary>
	[Display(Name = nameof(SharedResources.AUTHOR_BOOKS), ResourceType = typeof(SharedResources))]
	[JsonPropertyOrder(2)]
	public required List<BookSummaryContract> Books { get; init; }
	#endregion
}
