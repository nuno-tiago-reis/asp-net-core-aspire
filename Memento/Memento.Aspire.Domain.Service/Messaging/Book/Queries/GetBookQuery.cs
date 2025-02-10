namespace Memento.Aspire.Domain.Service.Messaging.Book.Queries;

using Memento.Aspire.Shared.Messaging.Messages;

/// <summary>
/// Implements the interface for the get book query.
/// </summary>
///
/// <seealso cref="Query{T}" />
public sealed record GetBookQuery : Query<GetBookQueryResult>
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the book identifier.
	/// </summary>
	public required Guid BookId { get; init; }
	#endregion
}
