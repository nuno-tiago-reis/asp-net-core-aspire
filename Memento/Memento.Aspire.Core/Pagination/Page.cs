namespace Memento.Aspire.Core.Pagination;

using System.Text.Json.Serialization;

/// <summary>
/// Implements the generic interface for a page that provides properties to paginate the queries.
/// </summary>
///
/// <typeparam name="T">The type.</typeparam>
[JsonConverter(typeof(PageJsonConverterFactory))]
public sealed class Page<T> : List<T>, IPage<T>
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the number of the current page.
	/// </summary>
	public int PageNumber { get; }

	/// <summary>
	/// Gets or sets the size of the current page.
	/// </summary>
	public int PageSize { get; }

	/// <summary>
	/// Gets or sets the total count of item pages.
	/// </summary>
	public int TotalPages { get; }

	/// <summary>
	/// Gets or sets the total count of items.
	/// </summary>
	public int TotalItems { get; }

	/// <summary>
	/// Gets or sets the parameter on which the items were ordered.
	/// </summary>
	public string? OrderBy { get; }

	/// <summary>
	/// Gets or sets the direction on which the items were ordered.
	/// </summary>
	public string? OrderDirection { get; }

	/// <summary>
	/// Gets or sets the total count of items.
	/// </summary>
	public T[] Items
	{
		get
		{
			return [ .. this ];
		}
	}
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="Page{T}"/> class.
	/// </summary>
	///
	/// <param name="items">The items.</param>
	/// <param name="totalItems">The total items.</param>
	/// <param name="totalPages">The total pages.</param>
	/// <param name="pageNumber">The page number.</param>
	/// <param name="pageSize">The page size.</param>
	/// <param name="orderBy">The parameter on which the results were ordered.</param>
	/// <param name="orderDirection">The direction on which the results were ordered.</param>
	internal Page(IList<T> items, int totalItems, int totalPages, int pageNumber, int pageSize, string? orderBy, string? orderDirection)
	{
		this.PageNumber = pageNumber;
		this.PageSize = pageSize;

		this.TotalPages = totalPages;
		this.TotalItems = totalItems;

		this.OrderBy = orderBy;
		this.OrderDirection = orderDirection;

		this.AddRange(items);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Page{T}"/> class.
	/// </summary>
	///
	/// <param name="items">The items.</param>
	/// <param name="totalItems">The total items.</param>
	/// <param name="pageNumber">The page number.</param>
	/// <param name="pageSize">The page size.</param>
	/// <param name="orderBy">The parameter on which the results were ordered.</param>
	/// <param name="orderDirection">The direction on which the results were ordered.</param>
	internal Page(IList<T> items, int totalItems, int pageNumber, int pageSize, string? orderBy, string? orderDirection)
	{
		this.PageNumber = pageNumber;
		this.PageSize = pageSize;

		this.TotalPages = Math.Max((totalItems / pageSize) + (totalItems % pageSize == 0 ? 0 : 1), 1);
		this.TotalItems = totalItems;

		this.OrderBy = orderBy;
		this.OrderDirection = orderDirection;

		this.AddRange(items);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Page{T}"/> class.
	/// </summary>
	public Page()
	{
		// Nothing to do here.
	}
	#endregion
}
