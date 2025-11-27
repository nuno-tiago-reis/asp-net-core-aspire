namespace Memento.Aspire.Domain.Service.Messaging.Author.Events;

using Memento.Aspire.Domain.Service.Contracts.Author;
using Memento.Aspire.Core.Messaging.Events;

/// <summary>
/// Implements the 'AuthorUpdated' event.
/// </summary>
public sealed record AuthorUpdatedEvent : Event
{
	#region [Properties]
	/// <summary>
	/// The author.
	/// </summary>
	public required AuthorDetailContract Author { get; init; }
	#endregion
}
