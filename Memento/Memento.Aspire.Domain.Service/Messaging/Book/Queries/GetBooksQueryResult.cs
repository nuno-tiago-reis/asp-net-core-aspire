namespace Memento.Aspire.Domain.Service.Messaging.Book.Queries;

using Memento.Aspire.Domain.Service.Contracts.Book;
using Memento.Aspire.Shared.Messaging.RequestResponse;
using Memento.Aspire.Shared.Pagination;

/// <summary>
/// Implements the interface for the get books query result.
/// </summary>
///
/// <seealso cref="QueryResult" />
public sealed record GetBooksQueryResult : QueryResult
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the book contracts.
	/// </summary>
	public required Page<BookSummaryContract>? BookContracts { get; init; }
	#endregion
}
