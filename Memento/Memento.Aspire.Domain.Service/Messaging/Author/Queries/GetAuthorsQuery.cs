namespace Memento.Aspire.Domain.Service.Messaging.Author.Queries;

using Memento.Aspire.Domain.Service.Contracts.Author;
using Memento.Aspire.Shared.Messaging.RequestResponse;

/// <summary>
/// Implements the interface for the get authors query.
/// </summary>
///
/// <seealso cref="Query{T}" />
public sealed record GetAuthorsQuery : Query<GetAuthorsQueryResult>
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the author filter contract.
	/// </summary>
	public required AuthorFilterContract AuthorFilterContract { get; set; }
	#endregion
}
