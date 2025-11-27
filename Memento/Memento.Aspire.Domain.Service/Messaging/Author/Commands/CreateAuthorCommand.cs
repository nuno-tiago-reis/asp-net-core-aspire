namespace Memento.Aspire.Domain.Service.Messaging.Author.Commands;

using Memento.Aspire.Domain.Service.Contracts.Author;
using Memento.Aspire.Core.Messaging.Messages;

/// <summary>
/// Implements the interface for the create author command.
/// </summary>
///
/// <seealso cref="Command{T}" />
public sealed record CreateAuthorCommand : Command<CreateAuthorCommandResult>
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the author contract.
	/// </summary>
	public required AuthorFormContract AuthorContract { get; init; }
	#endregion
}
