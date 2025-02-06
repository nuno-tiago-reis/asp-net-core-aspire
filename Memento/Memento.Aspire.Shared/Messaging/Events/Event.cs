namespace Memento.Aspire.Shared.Messaging.Events;

/// <summary>
/// Implements a generic event.
/// Provides properties to maintain traceability and configure messaging behaviour.
/// </summary>
public abstract record Event
{
	#region [Properties]
	/// <summary>
	/// The correlation identifier.
	/// </summary>
	public required Guid CorrelationId { get; init; }

	/// <summary>
	/// The timestamp.
	/// </summary>
	public required DateTimeOffset Timestamp { get; init; }
	#endregion
}
