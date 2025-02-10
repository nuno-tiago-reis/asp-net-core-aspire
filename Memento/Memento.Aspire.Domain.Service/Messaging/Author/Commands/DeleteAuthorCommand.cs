namespace Memento.Aspire.Domain.Service.Messaging.Author.Commands;

using Memento.Aspire.Shared.Messaging.Messages;

/// <summary>
/// Implements the interface for the delete author command.
/// </summary>
///
/// <seealso cref="Command{T}" />
public sealed record DeleteAuthorCommand : Command<DeleteAuthorCommandResult>
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the author identifier.
	/// </summary>
	public required Guid AuthorId { get; init; }
	#endregion
}
