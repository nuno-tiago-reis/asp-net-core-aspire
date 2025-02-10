namespace Memento.Aspire.Domain.Service.Messaging.Book.Queries;

using Memento.Aspire.Domain.Service.Contracts.Book;
using Memento.Aspire.Shared.Messaging.Messages;

/// <summary>
/// Implements the interface for the get books query.
/// </summary>
///
/// <seealso cref="Query{T}" />
public sealed record GetBooksQuery : Query<GetBooksQueryResult>
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the book filter contract.
	/// </summary>
	public required BookFilterContract BookFilterContract { get; init; }
	#endregion
}
