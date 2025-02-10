namespace Memento.Aspire.Domain.Service.Messaging.Genre.Queries;

using Memento.Aspire.Domain.Service.Contracts.Genre;
using Memento.Aspire.Shared.Messaging.Messages;
using Memento.Aspire.Shared.Pagination;

/// <summary>
/// Implements the interface for the get genres query result.
/// </summary>
///
/// <seealso cref="QueryResult" />
public sealed record GetGenresQueryResult : QueryResult
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the genre contracts.
	/// </summary>
	public required Page<GenreSummaryContract>? GenreContracts { get; init; }
	#endregion
}
