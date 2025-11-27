namespace Memento.Aspire.Domain.Service.Messaging.Author.Queries;

using Memento.Aspire.Domain.Service.Contracts.Author;
using Memento.Aspire.Core.Messaging.Messages;
using Memento.Aspire.Core.Pagination;

/// <summary>
/// Implements the interface for the get authors query result.
/// </summary>
///
/// <seealso cref="QueryResult" />
public sealed record GetAuthorsQueryResult : QueryResult
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the author contracts.
	/// </summary>
	public required Page<AuthorSummaryContract>? AuthorContracts { get; init; }
	#endregion
}
