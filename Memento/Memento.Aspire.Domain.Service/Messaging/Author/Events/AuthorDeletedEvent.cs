namespace Memento.Aspire.Domain.Service.Messaging.Author.Events;

using Memento.Aspire.Shared.Messaging.Events;

/// <summary>
/// Implements the 'AuthorDeleted' event.
/// </summary>
public sealed record AuthorDeletedEvent : Event
{
	#region [Properties]
	/// <summary>
	/// The author identifier.
	/// </summary>
	public required Guid AuthorId { get; set; }
	#endregion
}
