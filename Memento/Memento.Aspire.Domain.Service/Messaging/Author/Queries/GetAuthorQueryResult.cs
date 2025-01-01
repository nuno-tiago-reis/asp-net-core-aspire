namespace Memento.Aspire.Domain.Service.Messaging.Author.Queries;

using Memento.Aspire.Domain.Service.Contracts.Author;
using Memento.Aspire.Shared.Messaging.RequestResponse;

/// <summary>
/// Implements the interface for the get author query result.
/// </summary>
///
/// <seealso cref="QueryResult" />
public sealed record GetAuthorQueryResult : QueryResult
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the author contract.
	/// </summary>
	public required AuthorDetailContract? AuthorContract { get; set; }
	#endregion
}
