namespace Memento.Aspire.Domain.Service.Messaging.Book.Queries;

using Memento.Aspire.Domain.Service.Contracts.Book;
using Memento.Aspire.Shared.Messaging.RequestResponse;

/// <summary>
/// Implements the interface for the get book query result.
/// </summary>
///
/// <seealso cref="QueryResult" />
public sealed record GetBookQueryResult : QueryResult
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the book contract.
	/// </summary>
	public required BookDetailContract? BookContract { get; init; }
	#endregion
}
