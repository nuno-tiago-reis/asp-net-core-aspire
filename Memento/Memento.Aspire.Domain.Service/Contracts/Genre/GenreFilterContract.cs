namespace Memento.Aspire.Domain.Service.Contracts.Genre;

using Memento.Aspire.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

/// <summary>
/// Defines the fields over which 'Genres' can be filtered.
/// </summary>
///
/// <seealso cref="GenreOrderBy" />
/// <seealso cref="GenreOrderDirection" />
public sealed record GenreFilterContract : EntityFilterContract<GenreOrderBy, GenreOrderDirection>
{
	#region [Properties]
	/// <summary>
	/// The name filter.
	/// Only returns genres matching this value.
	/// </summary>
	public string? Name { get; set; }
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
	}
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
	/// By 'Name'.
	/// </summary>
	Name = 2
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
