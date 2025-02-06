namespace Memento.Aspire.Domain.Service.Messaging.Book.Events;

using Memento.Aspire.Domain.Service.Contracts.Book;
using Memento.Aspire.Shared.Messaging.Events;

/// <summary>
/// Implements the 'BookCreated' event.
/// </summary>
public sealed record BookCreatedEvent : Event
{
	#region [Properties]
	/// <summary>
	/// The book.
	/// </summary>
	public required BookDetailContract Book { get; init; }
	#endregion
}
