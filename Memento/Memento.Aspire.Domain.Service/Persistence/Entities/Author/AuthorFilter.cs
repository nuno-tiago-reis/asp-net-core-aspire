namespace Memento.Aspire.Domain.Service.Persistence.Entities.Author;

using Memento.Aspire.Shared.Persistence;

/// <summary>
/// Defines the fields over which 'Authors' can be filtered.
/// </summary>
///
/// <seealso cref="AuthorOrderBy" />
/// <seealso cref="AuthorOrderDirection" />
public sealed record AuthorFilter : EntityFilter<AuthorOrderBy, AuthorOrderDirection>
{
	#region [Properties]
	/// <summary>
	/// The name filter.
	/// Only returns authors matching this queryString.
	/// </summary>
	public string? Name { get; set; }

	/// <summary>
	/// The born before filter.
	/// Only returns authors with a birth date that is lesser or equal to this queryString.
	/// </summary>
	public DateOnly? BornBefore { get; set; }

	/// <summary>
	/// The born after filter.
	/// Only returns authors with a birth date that is greater or equal to this queryString.
	/// </summary>
	public DateOnly? BornAfter { get; set; }
	#endregion
}

/// <summary>
/// Defines the fields over which 'Authors' can be ordered by.
/// </summary>
public enum AuthorOrderBy
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
	/// By 'BirthDate'.
	/// </summary>
	BirthDate = 4
}

/// <summary>
/// Defines the direction over which 'Authors' can be ordered by.
/// </summary>
public enum AuthorOrderDirection
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
