namespace Memento.Aspire.Domain.Service.Messaging.Book.Commands;

using Memento.Aspire.Domain.Service.Contracts.Book;
using Memento.Aspire.Core.Messaging.Messages;

/// <summary>
/// Implements the interface for the update Book command.
/// </summary>
///
/// <seealso cref="Command{T}" />
public sealed record UpdateBookCommand : Command<UpdateBookCommandResult>
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the book identifier.
	/// </summary>
	public required Guid BookId { get; init; }

	/// <summary>
	/// Gets or sets the book contract.
	/// </summary>
	public required BookFormContract BookContract { get; init; }
	#endregion
}
