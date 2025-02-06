namespace Memento.Aspire.Domain.Service.Messaging.Book.Events;

using Memento.Aspire.Shared.Messaging.Events;

/// <summary>
/// Implements the 'BookDeleted' event.
/// </summary>
public sealed record BookDeletedEvent : Event
{
	#region [Properties]
	/// <summary>
	/// The book identifier.
	/// </summary>
	public required Guid BookId { get; init; }
	#endregion
}
