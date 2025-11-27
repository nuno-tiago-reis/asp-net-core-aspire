namespace Memento.Aspire.Core.Contracts;

using Memento.Aspire.Core.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Globalization;

/// <summary>
/// Implements a generic entity filter contract.
/// Provides properties to filter the entity queries.
/// </summary>
///
/// <typeparam name="TOrderBy">The entity filter order by type.</typeparam>
/// <typeparam name="TOrderDirection">The entity filter order direction type.</typeparam>
public abstract record EntityFilterContract<TOrderBy, TOrderDirection>
	where TOrderBy : struct, Enum
	where TOrderDirection : struct, Enum
{
	#region [Constants]
	/// <summary>
	/// The maximum page size.
	/// </summary>
	public const int MaximumPageSize = 50;

	/// <summary>
	/// The default page size.
	/// </summary>
	public const int DefaultPageSize = 10;

	/// <summary>
	/// The minimum page size.
	/// </summary>
	public const int MinimumPageSize = 1;
	#endregion

	#region [Attributes]
	/// <summary>
	/// The page number.
	/// </summary>
	private int InnerPageNumber;

	/// <summary>
	/// The page size.
	/// </summary>
	private int InnerPageSize;
	#endregion

	#region [Properties]
	/// <summary>
	/// Gets or sets the page number.
	/// </summary>
	public required int PageNumber
	{
		get
		{
			return this.InnerPageNumber;
		}
		set
		{
			this.InnerPageNumber = Math.Max(value, 1);
		}
	}

	/// <summary>
	/// Gets or sets the page size.
	/// </summary>
	public required int PageSize
	{
		get
		{
			return this.InnerPageSize;
		}
		set
		{
			this.InnerPageSize = Math.Min(Math.Max(value, MinimumPageSize), MaximumPageSize);
		}
	}

	/// <summary>
	/// Gets or sets the order by Value.
	/// </summary>
	public required TOrderBy OrderBy { get; set; }

	/// <summary>
	/// Gets or sets the order direction Value.
	/// </summary>
	public required TOrderDirection OrderDirection { get; set; }
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="EntityFilterContract{TOrderBy, TOrderDirection}"/> class.
	/// </summary>
	protected EntityFilterContract()
	{
		this.PageNumber = 1;
		this.PageSize = DefaultPageSize;
		this.OrderBy = default!;
		this.OrderDirection = default!;
	}
	#endregion

	#region [Methods]
	/// <summary>
	/// Reads the filter from a query string.
	/// </summary>
	///
	/// <param name="query">The query.</param>
	public virtual void ReadFromQuery(Dictionary<string, StringValues> query)
	{
		if (query is not null)
		{
			this.ReadFilterFromQuery(query);
			this.ReadPagingFromQuery(query);
			this.ReadOrderingFromQuery(query);
		}
	}

	/// <summary>
	/// Writes the filter to a query string.
	/// </summary>
	public virtual Dictionary<string, string> WriteToQuery()
	{
		var query = new Dictionary<string, string>();

		this.WriteFilterToQuery(query);
		this.WritePagingToQuery(query);
		this.WriteOrderingToQuery(query);

		return query;
	}

	/// <summary>
	/// Reads the filters remaining properties from a query string.
	/// </summary>
	///
	/// <param name="query">The query.</param>
	protected abstract void ReadFilterFromQuery(Dictionary<string, StringValues> query);

	/// <summary>
	/// Reads the filters paging properties from a query string.
	/// </summary>
	///
	/// <param name="query">The query.</param>
	protected virtual void ReadPagingFromQuery(Dictionary<string, StringValues> query)
	{
		// PageNumber
		if (query.TryGetValue(nameof(this.PageNumber), out var pageNumberQuery))
		{
			if (int.TryParse(pageNumberQuery, out var pageNumber))
			{
				this.PageNumber = pageNumber;
			}
		}

		// PageSize
		if (query.TryGetValue(nameof(this.PageSize), out var pageSizeQuery))
		{
			if (int.TryParse(pageSizeQuery, out var pageSize))
			{
				this.PageSize = pageSize;
			}
		}
	}

	/// <summary>
	/// Reads the filters ordering properties from a query string.
	/// </summary>
	///
	/// <param name="query">The query.</param>
	protected virtual void ReadOrderingFromQuery(Dictionary<string, StringValues> query)
	{
		// OrderBy
		if (query.TryGetValue(nameof(this.OrderBy), out var orderByQuery))
		{
			if (Enum.TryParse<TOrderBy>(orderByQuery, out var orderBy))
			{
				this.OrderBy = orderBy!;
			}
		}

		// OrderDirection
		if (query.TryGetValue(nameof(this.OrderDirection), out var orderDirectionQuery))
		{
			if (Enum.TryParse<TOrderDirection>(orderDirectionQuery, out var orderDirection))
			{
				this.OrderDirection = orderDirection!;
			}
		}
	}

	/// <summary>
	/// Writes the filters remaining properties to a query string.
	/// </summary>
	///
	/// <param name="query">The query.</param>
	protected abstract void WriteFilterToQuery(Dictionary<string, string> query);

	/// <summary>
	/// Writes the filters paging properties to a query string.
	/// </summary>
	///
	/// <param name="query">The query.</param>
	protected virtual void WritePagingToQuery(Dictionary<string, string> query)
	{
		// PageNumber
		query.Add(nameof(this.PageNumber), this.PageNumber.ToString(CultureInfo.InvariantCulture));

		// PageSize
		query.Add(nameof(this.PageSize), this.PageSize.ToString(CultureInfo.InvariantCulture));
	}

	/// <summary>
	/// Writes the filters ordering properties to a query string.
	/// </summary>
	///
	/// <param name="query">The query.</param>
	protected virtual void WriteOrderingToQuery(Dictionary<string, string> query)
	{
		// OrderBy
		query.Add(nameof(this.OrderBy), this.OrderBy.ToString()!);

		// OrderDirection
		query.Add(nameof(this.OrderDirection), this.OrderDirection.ToString()!);
	}
	#endregion
}
