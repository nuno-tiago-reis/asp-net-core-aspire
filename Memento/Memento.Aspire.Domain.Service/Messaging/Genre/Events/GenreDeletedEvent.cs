namespace Memento.Aspire.Domain.Service.Messaging.Genre.Events;

using Memento.Aspire.Domain.Service.Contracts.Genre;
using Memento.Aspire.Core.Messaging.Events;

/// <summary>
/// Implements the 'GenreDeleted' event.
/// </summary>
public sealed record GenreDeletedEvent : Event
{
	#region [Properties]
	/// <summary>
	/// The genre.
	/// </summary>
	public required GenreDetailContract Genre { get; init; }
	#endregion
}
