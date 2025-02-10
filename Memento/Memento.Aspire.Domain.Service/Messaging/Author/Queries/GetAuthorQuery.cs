namespace Memento.Aspire.Domain.Service.Messaging.Author.Queries;

using Memento.Aspire.Shared.Messaging.Messages;

/// <summary>
/// Implements the interface for the get author query.
/// </summary>
///
/// <seealso cref="Query{T}" />
public sealed record GetAuthorQuery : Query<GetAuthorQueryResult>
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the author identifier.
	/// </summary>
	public required Guid AuthorId { get; init; }
	#endregion
}
