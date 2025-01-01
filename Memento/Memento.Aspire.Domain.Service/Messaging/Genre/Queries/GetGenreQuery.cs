namespace Memento.Aspire.Domain.Service.Messaging.Genre.Queries;

using Memento.Aspire.Shared.Messaging.RequestResponse;

/// <summary>
/// Implements the interface for the get genre query.
/// </summary>
///
/// <seealso cref="Query{T}" />
public sealed record GetGenreQuery : Query<GetGenreQueryResult>
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the genre identifier.
	/// </summary>
	public required Guid GenreId { get; set; }
	#endregion
}
