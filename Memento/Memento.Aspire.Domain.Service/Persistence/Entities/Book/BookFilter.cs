namespace Memento.Aspire.Domain.Service.Persistence.Entities.Book;

using Memento.Aspire.Core.Persistence;

/// <summary>
/// Defines the fields over which 'Books' can be filtered.
/// </summary>
///
/// <seealso cref="BookOrderBy" />
/// <seealso cref="BookOrderDirection" />
public sealed record BookFilter : EntityFilter<BookOrderBy, BookOrderDirection>
{
	#region [Properties]
	/// <summary>
	/// The name filter.
	/// Only returns books matching this value.
	/// </summary>
	public string? Name { get; set; }

	/// <summary>
	/// The released before filter.
	/// Only returns authors with a birth date that is lesser or equal to this value.
	/// </summary>
	public DateOnly? ReleasedBefore { get; set; }

	/// <summary>
	/// The released after filter.
	/// Only returns authors with a birth date that is greater or equal to this value.
	/// </summary>
	public DateOnly? ReleasedAfter { get; set; }

	/// <summary>
	/// The author filter.
	/// Only returns books matching this value (either by id or by name).
	/// </summary>
	public string? Author { get; set; }

	/// <summary>
	/// The genre filter.
	/// Only returns books matching this value (either by id or by name).
	/// </summary>
	public string? Genre { get; set; }
	#endregion
}

/// <summary>
/// Defines the fields over which 'Books' can be ordered by.
/// </summary>
public enum BookOrderBy
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
	Name = 3,
	/// <summary>
	/// By 'ReleaseDate'.
	/// </summary>
	ReleaseDate = 4,
	/// <summary>
	/// By 'Author'.
	/// </summary>
	Author = 5,
	/// <summary>
	/// By 'Genre'.
	/// </summary>
	Genre = 6
}

/// <summary>
/// Defines the direction over which 'Books' can be ordered by.
/// </summary>
public enum BookOrderDirection
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
