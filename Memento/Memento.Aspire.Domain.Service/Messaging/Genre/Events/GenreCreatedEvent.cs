namespace Memento.Aspire.Domain.Service.Messaging.Genre.Events;

using Memento.Aspire.Domain.Service.Contracts.Genre;
using Memento.Aspire.Shared.Messaging.Events;

/// <summary>
/// Implements the 'GenreCreated' event.
/// </summary>
public sealed record GenreCreatedEvent : Event
{
	#region [Properties]
	/// <summary>
	/// The genre.
	/// </summary>
	public required GenreDetailContract Genre { get; set; }
	#endregion
}
