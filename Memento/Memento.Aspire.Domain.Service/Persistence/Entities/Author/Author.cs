namespace Memento.Aspire.Domain.Service.Persistence.Entities.Author;

using Memento.Aspire.Domain.Service.Persistence.Entities.Book;
using Memento.Aspire.Shared.Persistence;

/// <summary>
/// Implements the 'Author' entity.
/// </summary>
public sealed record Author : Entity
{
	#region [Properties] Stored
	/// <summary>
	/// The name.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// The birth date.
	/// </summary>
	public required DateOnly BirthDate { get; set; }
	#endregion

	#region [Properties] Navigation
	/// <summary>
	/// The books associated with the author.
	/// </summary>
	public required List<Book> Books { get; set; }
	#endregion
}
