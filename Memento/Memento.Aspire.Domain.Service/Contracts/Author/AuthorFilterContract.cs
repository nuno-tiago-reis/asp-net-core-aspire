namespace Memento.Aspire.Domain.Service.Contracts.Author;

using Memento.Aspire.Shared.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

/// <summary>
/// Defines the fields over which 'Authors' can be filtered.
/// </summary>
///
/// <seealso cref="AuthorOrderBy" />
/// <seealso cref="AuthorOrderDirection" />
public sealed record AuthorFilterContract : EntityFilterContract<AuthorOrderBy, AuthorOrderDirection>
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

		// BornBefore
		if (query.TryGetValue(nameof(this.BornBefore), out var bornBeforeQuery))
		{
			if (DateOnly.TryParse(bornBeforeQuery, out var bornBefore))
			{
				this.BornBefore = bornBefore;
			}
		}

		// BornAfter
		if (query.TryGetValue(nameof(this.BornAfter), out var bornAfterQuery))
		{
			if (DateOnly.TryParse(bornAfterQuery, out var bornAfter))
			{
				this.BornAfter = bornAfter;
			}
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

		// BornBefore
		if (this.BornBefore is not null)
		{
			query.Add(nameof(this.BornBefore), this.BornBefore.Value.ToShortDateString());
		}

		// BornAfter
		if (this.BornAfter is not null)
		{
			query.Add(nameof(this.BornAfter), this.BornAfter.Value.ToShortDateString());
		}
	}
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
	/// By 'Name'.
	/// </summary>
	Name = 1,
	/// <summary>
	/// By 'BirthDate'.
	/// </summary>
	BirthDate = 2
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
