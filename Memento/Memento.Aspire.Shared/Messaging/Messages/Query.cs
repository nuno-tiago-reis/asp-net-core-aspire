namespace Memento.Aspire.Shared.Messaging.Messages;

/// <summary>
/// Implements a generic query.
/// Provides properties to maintain traceability and configure messaging behaviour.
/// </summary>
public abstract record Query<TQueryResult> : Message<TQueryResult> where TQueryResult : QueryResult
{
	// Intentionally Empty.
}
