namespace Memento.Aspire.Domain.Service.Messaging.Author.Commands;

using Memento.Aspire.Domain.Service.Contracts.Author;
using Memento.Aspire.Shared.Messaging.Messages;

/// <summary>
/// Implements the interface for the update author command.
/// </summary>
///
/// <seealso cref="Command{T}" />
public sealed record UpdateAuthorCommand : Command<UpdateAuthorCommandResult>
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the author identifier.
	/// </summary>
	public required Guid AuthorId { get; init; }

	/// <summary>
	/// Gets or sets the author contract.
	/// </summary>
	public required AuthorFormContract AuthorContract { get; init; }
	#endregion
}
