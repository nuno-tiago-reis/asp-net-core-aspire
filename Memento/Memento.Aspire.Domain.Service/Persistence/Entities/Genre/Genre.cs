namespace Memento.Aspire.Domain.Service.Persistence.Entities.Genre;

using Memento.Aspire.Domain.Service.Persistence.Entities.Book;
using Memento.Aspire.Core.Persistence;
using Memento.Aspire.Core.Resources;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Implements the 'Genre' entity.
/// </summary>
public sealed record Genre : Entity
{
	#region [Properties] Stored
	/// <summary>
	/// The name.
	/// </summary>
	[Display(Name = nameof(SharedResources.GENRE_NAME), ResourceType = typeof(SharedResources))]
	public required string Name { get; set; }
	#endregion

	#region [Properties] Navigation
	/// <summary>
	/// The books associated with the genre.
	/// </summary>
	public required List<Book> Books { get; set; }
	#endregion
}
