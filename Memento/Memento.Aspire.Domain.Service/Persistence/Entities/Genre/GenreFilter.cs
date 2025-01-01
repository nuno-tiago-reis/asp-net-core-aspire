namespace Memento.Aspire.Domain.Service.Persistence.Entities.Genre;

using Memento.Aspire.Shared.Persistence;

/// <summary>
/// Defines the fields over which 'Genres' can be filtered.
/// </summary>
///
/// <seealso cref="GenreOrderBy" />
/// <seealso cref="GenreOrderDirection" />
public sealed record GenreFilter : EntityFilter<GenreOrderBy, GenreOrderDirection>
{
	#region [Properties]
	/// <summary>
	///  The name filter.
	///  Only returns genres matching this value.
	/// </summary>
	public string? Name { get; set; }
	#endregion
}

/// <summary>
/// Defines the fields over which 'Genres' can be ordered by.
/// </summary>
public enum GenreOrderBy
{
	/// <summary>
	/// By 'Id'.
	/// </summary>
	Id = 0,
	/// <summary>
	/// By 'CreatedAt'.
	/// </summary>
	CreatedAt = 1,
	/// <summary>
	/// By 'UpdatedAt'.
	/// </summary>
	UpdatedAt = 2,

	/// <summary>
	/// By 'Name'.
	/// </summary>
	Name = 3
}

/// <summary>
/// Defines the direction over which 'Genres' can be ordered by.
/// </summary>
public enum GenreOrderDirection
{
	/// <summary>
	/// In 'Ascending' order.
	/// </summary>
	Ascending = 0,
	/// <summary>
	/// In 'Descending' order.
	/// </summary>
	Descending = 1
}
