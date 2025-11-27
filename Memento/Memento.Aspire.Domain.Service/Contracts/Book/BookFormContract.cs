namespace Memento.Aspire.Domain.Service.Contracts.Book;

using Memento.Aspire.Core.Resources;
using System;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Implements the 'Book' form contract.
/// </summary>
public sealed record BookFormContract
{
	#region [Properties]
	/// <summary>
	/// The book's name.
	/// </summary>
	[Display(Name = nameof(SharedResources.BOOK_NAME), ResourceType = typeof(SharedResources))]
	public required string? Name { get; init; }

	/// <summary>
	/// The book's release date.
	/// </summary>
	[Display(Name = nameof(SharedResources.BOOK_RELEASEDATE), ResourceType = typeof(SharedResources))]
	public required DateOnly? ReleaseDate { get; init; }

	/// <summary>
	/// The Persons associated with the Book.
	/// </summary>
	[Display(Name = nameof(SharedResources.BOOK_AUTHOR), ResourceType = typeof(SharedResources))]
	public required Guid? AuthorId { get; init; }

	/// <summary>
	/// The genres associated with the Book.
	/// </summary>
	[Display(Name = nameof(SharedResources.BOOK_GENRE), ResourceType = typeof(SharedResources))]
	public required Guid? GenreId { get; init; }
	#endregion
}
