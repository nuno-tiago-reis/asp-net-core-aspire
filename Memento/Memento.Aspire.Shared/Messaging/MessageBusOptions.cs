namespace Memento.Aspire.Shared.Messaging;

/// <summary>
/// Implements the 'MessageBus' options.
/// </summary>
public sealed record MessageBusOptions
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the connection string.
	/// </summary>
	public required string? ConnectionString { get; init; }
	#endregion
}
