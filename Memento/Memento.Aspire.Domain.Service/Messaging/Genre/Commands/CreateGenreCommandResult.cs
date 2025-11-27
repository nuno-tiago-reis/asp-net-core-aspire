namespace Memento.Aspire.Domain.Service.Messaging.Genre.Commands;

using Memento.Aspire.Domain.Service.Contracts.Genre;
using Memento.Aspire.Core.Messaging.Messages;

/// <summary>
/// Implements the interface for the create genre command result.
/// </summary>
///
/// <seealso cref="CommandResult" />
public sealed record CreateGenreCommandResult : CommandResult
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the genre contract.
	/// </summary>
	public required GenreDetailContract? GenreContract { get; init; }
	#endregion
}
