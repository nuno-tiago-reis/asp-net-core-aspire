namespace Memento.Aspire.Domain.Service.Messaging.Genre.Queries;

using Memento.Aspire.Domain.Service.Contracts.Genre;
using Memento.Aspire.Shared.Messaging.Messages;

/// <summary>
/// Implements the interface for the get genres query.
/// </summary>
///
/// <seealso cref="Query{T}" />
public sealed record GetGenresQuery : Query<GetGenresQueryResult>
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the genre filter contract.
	/// </summary>
	public required GenreFilterContract GenreFilterContract { get; init; }
	#endregion
}
