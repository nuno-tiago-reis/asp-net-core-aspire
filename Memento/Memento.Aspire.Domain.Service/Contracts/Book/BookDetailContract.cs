namespace Memento.Aspire.Domain.Service.Contracts.Book;

using Memento.Aspire.Domain.Service.Contracts.Author;
using Memento.Aspire.Domain.Service.Contracts.Genre;
using Memento.Aspire.Shared.Contracts;
using Memento.Aspire.Shared.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

/// <summary>
/// Implements the 'Book' detail contract.
/// </summary>
public sealed record BookDetailContract : EntityContract
{
	#region [Properties]
	/// <summary>
	/// The book's name.
	/// </summary>
	[Display(Name = nameof(SharedResources.BOOK_NAME), ResourceType = typeof(SharedResources))]
	[JsonPropertyOrder(0)]
	public required string Name { get; set; }

	/// <summary>
	/// The book's release date.
	/// </summary>
	[Display(Name = nameof(SharedResources.BOOK_RELEASEDATE), ResourceType = typeof(SharedResources))]
	[JsonPropertyOrder(1)]
	public required DateOnly ReleaseDate { get; set; }

	/// <summary>
	/// The book's author.
	/// </summary>
	[Display(Name = nameof(SharedResources.BOOK_AUTHOR), ResourceType = typeof(SharedResources))]
	[JsonPropertyOrder(2)]
	public required AuthorSummaryContract Author { get; set; }

	/// <summary>
	/// The book's genre.
	/// </summary>
	[Display(Name = nameof(SharedResources.BOOK_GENRE), ResourceType = typeof(SharedResources))]
	[JsonPropertyOrder(3)]
	public required GenreSummaryContract Genre { get; set; }
	#endregion
}
