namespace Memento.Aspire.Domain.Service.Messaging.Author.Events;

using Memento.Aspire.Domain.Service.Contracts.Author;
using Memento.Aspire.Core.Messaging.Events;

/// <summary>
/// Implements the 'AuthorDeleted' event.
/// </summary>
public sealed record AuthorDeletedEvent : Event
{
	#region [Properties]
	/// <summary>
	/// The author.
	/// </summary>
	public required AuthorDetailContract Author { get; init; }
	#endregion
}
