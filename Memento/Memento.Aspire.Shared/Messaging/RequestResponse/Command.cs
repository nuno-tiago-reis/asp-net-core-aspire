namespace Memento.Aspire.Shared.Messaging.RequestResponse;

/// <summary>
/// Implements a generic command.
/// Provides properties to maintain traceability and configure messaging behaviour.
/// </summary>
public abstract record Command<TCommandResult> : Message<TCommandResult> where TCommandResult : CommandResult
{
	// Intentionally Empty.
}
