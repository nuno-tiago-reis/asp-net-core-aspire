namespace Memento.Aspire.Domain.Service.Contracts.Book;

using Memento.Aspire.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

/// <summary>
/// Defines the fields over which 'Books' can be filtered.
/// </summary>
///
/// <seealso cref="BookOrderBy" />
/// <seealso cref="BookOrderDirection" />
public sealed record BookFilterContract : EntityFilterContract<BookOrderBy, BookOrderDirection>
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

	#region [Methods]
	/// <summary>
	/// Reads the filters remaining properties from a query string.
	/// </summary>
	///
	/// <param name="query">The query.</param>
	protected override void ReadFilterFromQuery(Dictionary<string, StringValues> query)
	{
		// Name
		if (query.TryGetValue(nameof(this.Name), out var name))
		{
			this.Name = name;
		}

		// ReleasedBefore
		if (query.TryGetValue(nameof(this.ReleasedBefore), out var releasedBeforeQuery))
		{
			if (DateOnly.TryParse(releasedBeforeQuery, out var releasedBefore))
			{
				this.ReleasedBefore = releasedBefore;
			}
		}

		// ReleasedAfter
		if (query.TryGetValue(nameof(this.ReleasedAfter), out var releasedAfterQuery))
		{
			if (DateOnly.TryParse(releasedAfterQuery, out var releasedAfter))
			{
				this.ReleasedAfter = releasedAfter;
			}
		}

		// Author
		if (query.TryGetValue(nameof(this.Author), out var author))
		{
			this.Author = author;
		}

		// Genre
		if (query.TryGetValue(nameof(this.Genre), out var genre))
		{
			this.Genre = genre;
		}
	}

	/// <summary>
	/// Writes the filters remaining properties to a query string.
	/// </summary>
	///
	/// <param name="query">The query.</param>
	/// <inheritdoc />
	protected override void WriteFilterToQuery(Dictionary<string, string> query)
	{
		// Name
		if (!string.IsNullOrWhiteSpace(this.Name))
		{
			query.Add(nameof(this.Name), this.Name);
		}

		// ReleasedBefore
		if (this.ReleasedBefore is not null)
		{
			query.Add(nameof(this.ReleasedBefore), this.ReleasedBefore.Value.ToShortDateString());
		}

		// ReleasedAfter
		if (this.ReleasedAfter is not null)
		{
			query.Add(nameof(this.ReleasedAfter), this.ReleasedAfter.Value.ToShortDateString());
		}

		// Author
		if (!string.IsNullOrWhiteSpace(this.Author))
		{
			query.Add(nameof(this.Author), this.Author);
		}

		// Genre
		if (!string.IsNullOrWhiteSpace(this.Genre))
		{
			query.Add(nameof(this.Genre), this.Genre);
		}
	}
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
	/// By 'Name'.
	/// </summary>
	Name = 1,
	/// <summary>
	/// By 'ReleaseDate'.
	/// </summary>
	ReleaseDate = 2
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
