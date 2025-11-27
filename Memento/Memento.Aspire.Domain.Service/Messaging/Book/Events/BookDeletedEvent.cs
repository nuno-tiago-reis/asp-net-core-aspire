namespace Memento.Aspire.Domain.Service.Messaging.Book.Events;

using Memento.Aspire.Domain.Service.Contracts.Book;
using Memento.Aspire.Core.Messaging.Events;

/// <summary>
/// Implements the 'BookDeleted' event.
/// </summary>
public sealed record BookDeletedEvent : Event
{
	#region [Properties]
	/// <summary>
	/// The book.
	/// </summary>
	public required BookDetailContract Book { get; init; }
	#endregion
}
