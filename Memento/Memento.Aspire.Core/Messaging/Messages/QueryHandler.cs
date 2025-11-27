namespace Memento.Aspire.Core.Messaging.Messages;

using Microsoft.Extensions.Logging;

/// <summary>
/// Implements a generic query handler.
/// Provides methods to handle querys and query results.
/// </summary>
public abstract class QueryHandler<TQuery, TQueryResult> : MessageHandler<TQuery, TQueryResult>
	where TQuery : Query<TQueryResult>
	where TQueryResult : QueryResult
{
	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="QueryHandler{TQuery, TQueryResult}"/> class.
	/// </summary>
	///
	/// <param name="logger">The logger.</param>
	protected QueryHandler(ILogger<QueryHandler<TQuery, TQueryResult>> logger) : base(logger)
	{
		// Intentionally Empty.
	}
	#endregion
}
