namespace Memento.Aspire.Domain.Service.Messaging.Genre.Queries;

using Memento.Aspire.Domain.Service.Contracts.Genre;
using Memento.Aspire.Shared.Messaging.RequestResponse;

/// <summary>
/// Implements the interface for the get genre query result.
/// </summary>
///
/// <seealso cref="QueryResult" />
public sealed record GetGenreQueryResult : QueryResult
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the genre contract.
	/// </summary>
	public required GenreDetailContract? GenreContract { get; init; }
	#endregion
}
