namespace Memento.Aspire.Shared.Messaging.RequestResponse;

using Memento.Aspire.Shared.Exceptions;

/// <summary>
/// Implements a generic message result.
/// Provides properties to maintain traceability and configure messaging behaviour.
/// </summary>
public abstract record MessageResult
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

	/// <summary>
	/// Gets or sets whether the messages processing was successful.
	/// </summary>
	public required bool Success { get; init; }

	/// <summary>
	/// Gets or sets the exception.
	/// </summary>
	public required StandardException? Exception { get; init; }
	#endregion
}
