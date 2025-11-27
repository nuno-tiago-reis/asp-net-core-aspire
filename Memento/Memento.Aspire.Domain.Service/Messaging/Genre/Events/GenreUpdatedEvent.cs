namespace Memento.Aspire.Domain.Service.Messaging.Genre.Events;

using Memento.Aspire.Domain.Service.Contracts.Genre;
using Memento.Aspire.Core.Messaging.Events;

/// <summary>
/// Implements the 'GenreUpdated' event.
/// </summary>
public sealed record GenreUpdatedEvent : Event
{
	#region [Properties]
	/// <summary>
	/// The genre.
	/// </summary>
	public required GenreDetailContract Genre { get; init; }
	#endregion
}
