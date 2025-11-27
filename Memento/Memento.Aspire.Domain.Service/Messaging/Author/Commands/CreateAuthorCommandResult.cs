namespace Memento.Aspire.Domain.Service.Messaging.Author.Commands;

using Memento.Aspire.Domain.Service.Contracts.Author;
using Memento.Aspire.Core.Messaging.Messages;

/// <summary>
/// Implements the interface for the create author command result.
/// </summary>
///
/// <seealso cref="CommandResult{T}" />
public sealed record CreateAuthorCommandResult : CommandResult
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the author contract.
	/// </summary>
	public required AuthorDetailContract? AuthorContract { get; init; }
	#endregion
}
