namespace Memento.Aspire.Shared.Messaging.RequestResponse;

/// <summary>
/// Implements a generic command result.
/// Provides properties to maintain traceability and configure messaging behaviour.
/// </summary>
public abstract record CommandResult : MessageResult
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the idempotency identifier.
	/// </summary>
	public required Guid IdempotencyId { get; set; }
	#endregion
}
