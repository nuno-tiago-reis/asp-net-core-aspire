namespace Memento.Aspire.Domain.Service.Messaging.Genre.Commands;

using Memento.Aspire.Core.Messaging.Messages;

/// <summary>
/// Implements the interface for the delete genre command.
/// </summary>
///
/// <seealso cref="Command{T}" />
public sealed record DeleteGenreCommand : Command<DeleteGenreCommandResult>
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the genre identifier.
	/// </summary>
	public required Guid GenreId { get; init; }
	#endregion
}
