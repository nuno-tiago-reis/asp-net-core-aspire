﻿namespace Memento.Aspire.Domain.Service.Messaging.Book.Events;

using Memento.Aspire.Domain.Service.Contracts.Book;
using Memento.Aspire.Shared.Messaging.Events;

/// <summary>
/// Implements the 'BookUpdated' event.
/// </summary>
public sealed record BookUpdatedEvent : Event
{
	#region [Properties]
	/// <summary>
	/// The book.
	/// </summary>
	public required BookDetailContract Book { get; set; }
	#endregion
}
