namespace Memento.Aspire.Domain.Service.Messaging.Book.Commands;

using Memento.Aspire.Domain.Service.Contracts.Book;
using Memento.Aspire.Shared.Messaging.RequestResponse;

/// <summary>
/// Implements the interface for the update Book command result.
/// </summary>
///
/// <seealso cref="CommandResult" />
public sealed record UpdateBookCommandResult : CommandResult
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the contract.
	/// </summary>
	public required BookDetailContract? BookContract { get; init; }
	#endregion
}
