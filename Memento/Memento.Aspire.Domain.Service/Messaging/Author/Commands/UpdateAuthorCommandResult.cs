namespace Memento.Aspire.Domain.Service.Messaging.Author.Commands;

using Memento.Aspire.Domain.Service.Contracts.Author;
using Memento.Aspire.Shared.Messaging.RequestResponse;

/// <summary>
/// Implements the interface for the update author command result.
/// </summary>
///
/// <seealso cref="CommandResult" />
public sealed record UpdateAuthorCommandResult : CommandResult
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the contract.
	/// </summary>
	public required AuthorDetailContract? AuthorContract { get; init; }
	#endregion
}
