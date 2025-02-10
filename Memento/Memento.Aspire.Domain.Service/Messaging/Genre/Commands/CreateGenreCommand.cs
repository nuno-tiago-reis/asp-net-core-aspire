namespace Memento.Aspire.Domain.Service.Messaging.Genre.Commands;

using Memento.Aspire.Domain.Service.Contracts.Genre;
using Memento.Aspire.Shared.Messaging.Messages;

/// <summary>
/// Implements the interface for the create genre command.
/// </summary>
///
/// <seealso cref="Command{T}" />
public sealed record CreateGenreCommand : Command<CreateGenreCommandResult>
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the genre contract.
	/// </summary>
	public required GenreFormContract GenreContract { get; init; }
	#endregion
}
