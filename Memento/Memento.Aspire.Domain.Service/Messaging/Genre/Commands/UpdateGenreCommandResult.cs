namespace Memento.Aspire.Domain.Service.Messaging.Genre.Commands;

using Memento.Aspire.Domain.Service.Contracts.Genre;
using Memento.Aspire.Shared.Messaging.RequestResponse;

/// <summary>
/// Implements the interface for the update genre command result.
/// </summary>
///
/// <seealso cref="CommandResult" />
public sealed record UpdateGenreCommandResult : CommandResult
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the contract.
	/// </summary>
	public required GenreDetailContract? GenreContract { get; set; }
	#endregion
}
