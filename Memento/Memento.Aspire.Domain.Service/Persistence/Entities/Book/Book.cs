namespace Memento.Aspire.Domain.Service.Persistence.Entities.Book;

using Memento.Aspire.Domain.Service.Persistence.Entities.Author;
using Memento.Aspire.Domain.Service.Persistence.Entities.Genre;
using Memento.Aspire.Shared.Persistence;

/// <summary>
/// Implements the 'Book' entity.
/// </summary>
public sealed record Book : Entity
{
	#region [Properties] Stored
	/// <summary>
	/// The name.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// The release date.
	/// </summary>
	public required DateOnly ReleaseDate { get; set; }

	/// <summary>
	/// The author identifier.
	/// </summary>
	public required Guid AuthorId { get; set; }

	/// <summary>
	/// The genre identifier.
	/// </summary>
	public required Guid GenreId { get; set; }
	#endregion

	#region [Properties] Navigation
	/// <summary>
	/// The author navigation property.
	/// </summary>
	public Author? Author { get; set; }

	/// <summary>
	/// The genre navigation property.
	/// </summary>
	public Genre? Genre { get; set; }
	#endregion
}
