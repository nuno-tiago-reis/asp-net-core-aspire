namespace Memento.Aspire.Domain.Service.Messaging.Genre.Commands;

using Memento.Aspire.Domain.Service.Contracts.Genre;
using Memento.Aspire.Shared.Messaging.Messages;

/// <summary>
/// Implements the interface for the update genre command.
/// </summary>
///
/// <seealso cref="Command{T}" />
public sealed record UpdateGenreCommand : Command<UpdateGenreCommandResult>
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the genre identifier.
	/// </summary>
	public required Guid GenreId { get; init; }

	/// <summary>
	/// Gets or sets the identifier.
	/// </summary>
	/// <summary>
	/// Gets or sets the contract.
	/// </summary>
	public required GenreFormContract GenreContract { get; init; }
	#endregion
}
