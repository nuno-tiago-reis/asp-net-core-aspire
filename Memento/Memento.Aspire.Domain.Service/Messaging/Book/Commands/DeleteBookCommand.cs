namespace Memento.Aspire.Domain.Service.Messaging.Book.Commands;

using Memento.Aspire.Shared.Messaging.RequestResponse;

/// <summary>
/// Implements the interface for the delete Book command.
/// </summary>
///
/// <seealso cref="Command{T}" />
public sealed record DeleteBookCommand : Command<DeleteBookCommandResult>
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the book identifier.
	/// </summary>
	public required Guid BookId { get; set; }
	#endregion
}
