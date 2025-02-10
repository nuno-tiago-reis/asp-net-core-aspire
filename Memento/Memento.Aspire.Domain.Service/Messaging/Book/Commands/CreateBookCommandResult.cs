namespace Memento.Aspire.Domain.Service.Messaging.Book.Commands;

using Memento.Aspire.Domain.Service.Contracts.Book;
using Memento.Aspire.Shared.Messaging.Messages;

/// <summary>
/// Implements the interface for the create Book command result.
/// </summary>
///
/// <seealso cref="CommandResult" />
public sealed record CreateBookCommandResult : CommandResult
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the book contract.
	/// </summary>
	public required BookDetailContract? BookContract { get; init; }
	#endregion
}
