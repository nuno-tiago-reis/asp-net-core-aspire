namespace Memento.Aspire.Shared.Messaging.RequestResponse;

using MassTransit.Mediator;

/// <summary>
/// Implements a generic message.
/// Provides properties to maintain traceability and configure messaging behaviour.
/// </summary>
public abstract record Message<TMessageResult> : Request<TMessageResult> where TMessageResult : MessageResult
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the correlation identifier.
	/// </summary>
	public required Guid CorrelationId { get; init; }

	/// <summary>
	/// Gets or sets the user identifier.
	/// </summary>
	public required Guid UserId { get; init; }
	#endregion
}
