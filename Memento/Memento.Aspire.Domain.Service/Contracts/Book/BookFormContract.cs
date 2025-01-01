namespace Memento.Aspire.Domain.Service.Contracts.Book;

using Memento.Aspire.Shared.Resources;
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
	public required string? Name { get; set; }

	/// <summary>
	/// The book's release date.
	/// </summary>
	[Required]
	[Display(Name = nameof(SharedResources.BOOK_RELEASEDATE), ResourceType = typeof(SharedResources))]
	public required DateOnly? ReleaseDate { get; set; }

	/// <summary>
	/// The Persons associated with the Book.
	/// </summary>
	[Display(Name = nameof(SharedResources.BOOK_AUTHOR), ResourceType = typeof(SharedResources))]
	public required Guid? AuthorId { get; set; }

	/// <summary>
	/// The genres associated with the Book.
	/// </summary>
	[Display(Name = nameof(SharedResources.BOOK_GENRE), ResourceType = typeof(SharedResources))]
	public required Guid? GenreId { get; set; }
	#endregion
}
