namespace Memento.Aspire.Domain.Service.Messaging.Genre.Events;

using Memento.Aspire.Shared.Messaging.Events;

/// <summary>
/// Implements the 'GenreDeleted' event.
/// </summary>
public sealed record GenreDeletedEvent : Event
{
	#region [Properties]
	/// <summary>
	/// The genre identifier.
	/// </summary>
	public required Guid GenreId { get; set; }
	#endregion
}
