namespace Memento.Aspire.Shared.Contracts;

using Memento.Aspire.Shared.Binding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

/// <summary>
/// Implements a generic entity filter contract.
/// Provides properties to filter the entity queries.
/// </summary>
///
/// <typeparam name="TOrderBy">The entity filter order by type.</typeparam>
/// <typeparam name="TOrderDirection">The entity filter order direction type.</typeparam>
public abstract record EntityFilterContract<TOrderBy, TOrderDirection>
	where TOrderBy : Enum
	where TOrderDirection : Enum
{
	#region [Constants]
	/// <summary>
	/// The maximum page size.
	/// </summary>
	public const int MAXIMUM_PAGE_SIZE = 50;

	/// <summary>
	/// The default page size.
	/// </summary>
	public const int DEFAULT_PAGE_SIZE = 10;

	/// <summary>
	/// The minimum page size.
	/// </summary>
	public const int MINIMUM_PAGE_SIZE = 1;
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
	[FromQuery]
	public required int PageNumber
	{
		get { return this.InnerPageNumber; }
		set { this.InnerPageNumber = Math.Max(value, 1); }
	}

	/// <summary>
	/// Gets or sets the page size.
	/// </summary>
	[FromQuery]
	public required int PageSize
	{
		get { return this.InnerPageSize; }
		set { this.InnerPageSize = Math.Min(Math.Max(value, MINIMUM_PAGE_SIZE), MAXIMUM_PAGE_SIZE); }
	}

	/// <summary>
	/// Gets or sets the order by Value.
	/// </summary>
	[FromQuery]
	public required ParameterBinder<TOrderBy> OrderBy { get; set; }

	/// <summary>
	/// Gets or sets the order direction Value.
	/// </summary>
	[FromQuery]
	public required ParameterBinder<TOrderDirection> OrderDirection { get; set; }
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="EntityFilterContract{TOrderBy, TOrderDirection}"/> class.
	/// </summary>
	protected EntityFilterContract()
	{
		this.PageNumber = 1;
		this.PageSize = DEFAULT_PAGE_SIZE;
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
			if (ParameterBinder<TOrderBy>.TryParse(orderByQuery, out var orderBy))
			{
				this.OrderBy = orderBy;
			}
		}

		// OrderDirection
		if (query.TryGetValue(nameof(this.OrderDirection), out var orderDirectionQuery))
		{
			if (ParameterBinder<TOrderDirection>.TryParse(orderDirectionQuery, out var orderDirection))
			{
				this.OrderDirection = orderDirection;
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
		query.Add(nameof(this.PageNumber), this.PageNumber.ToString());

		// PageSize
		query.Add(nameof(this.PageSize), this.PageSize.ToString());
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
