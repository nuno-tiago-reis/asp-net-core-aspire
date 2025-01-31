﻿namespace Memento.Aspire.Domain.Service.Messaging.Author.Events;

using Memento.Aspire.Domain.Service.Contracts.Author;
using Memento.Aspire.Shared.Messaging.Events;

/// <summary>
/// Implements the 'AuthorCreated' event.
/// </summary>
public sealed record AuthorCreatedEvent : Event
{
	#region [Properties]
	/// <summary>
	/// The author.
	/// </summary>
	public required AuthorDetailContract Author { get; set; }
	#endregion
}
