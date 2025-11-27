namespace Memento.Aspire.Core.Pagination;

using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

/// <summary>
/// Implements the generic interface for a page that provides properties to paginate the queries.
/// </summary>
///
/// <typeparam name="T">The type.</typeparam>
[JsonConverter(typeof(PageJsonConverterFactory))]
public static class PageFactory
{
	#region [Methods]
	/// <summary>
	/// Creates a new instance of the <see cref="Page{T}"/> class.
	/// </summary>
	///
	/// <param name="enumerable">The enumerable.</param>
	/// <param name="enumerableCount">The enumerable count.</param>
	/// <param name="pageNumber">The page number.</param>
	/// <param name="pageSize">The page size.</param>
	/// <param name="orderBy">The parameter on which the results were ordered.</param>
	/// <param name="orderDirection">The direction on which the results were ordered.</param>
	public static Page<T> Create<T>
	(
		IList<T> enumerable,
		IList<T> enumerableCount,
		int pageNumber,
		int pageSize,
		string orderBy,
		string orderDirection
	)
	{
		var items = enumerable.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

		return new Page<T>(items, enumerableCount.Count, pageNumber, pageSize, orderBy, orderDirection);
	}

	/// <summary>
	/// Creates a new instance of the <see cref="Page{T}"/> class asynchronously.
	/// </summary>
	///
	/// <param name="queryable">The queryable.</param>
	/// <param name="queryableCount">The queryable count.</param>
	/// <param name="pageNumber">The page number.</param>
	/// <param name="pageSize">The page size.</param>
	/// <param name="orderBy">The parameter on which the results were ordered.</param>
	/// <param name="orderDirection">The direction on which the results were ordered.</param>
	public static async Task<Page<T>> CreateAsync<T>
	(
		IQueryable<T> queryable,
		IQueryable<T> queryableCount,
		int pageNumber,
		int pageSize,
		string orderBy,
		string orderDirection
	)
	{
		var items = await queryable.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

		return new Page<T>(items, await queryableCount.CountAsync(), pageNumber, pageSize, orderBy, orderDirection);
	}

	/// <summary>
	/// Creates a new instance of the <see cref="Page{T}"/> class without modifying the enumerable.
	/// </summary>
	///
	/// <param name="enumerable">The enumerable.</param>
	/// <param name="enumerableCount">The enumerable count.</param>
	/// <param name="pageNumber">The page number.</param>
	/// <param name="pageSize">The page size.</param>
	/// <param name="orderBy">The parameter on which the results were ordered.</param>
	/// <param name="orderDirection">The direction on which the results were ordered.</param>
	public static Page<T> CreateUnmodified<T>
	(
		IList<T> enumerable,
		int enumerableCount,
		int pageNumber,
		int pageSize,
		string orderBy,
		string orderDirection
	)
	{
		var items = enumerable.ToList();

		return new Page<T>(items, enumerableCount, pageNumber, pageSize, orderBy, orderDirection);
	}

	/// <summary>
	/// Creates a new instance of the <see cref="Page{T}"/> class without modifying the enumerable.
	/// </summary>
	///
	/// <param name="queryable">The queryable.</param>
	/// <param name="queryableCount">The queryable count.</param>
	/// <param name="pageNumber">The page number.</param>
	/// <param name="pageSize">The page size.</param>
	/// <param name="orderBy">The parameter on which the results were ordered.</param>
	/// <param name="orderDirection">The direction on which the results were ordered.</param>
	public static async Task<Page<T>> CreateUnmodifiedAsync<T>
	(
		IQueryable<T> queryable,
		int queryableCount,
		int pageNumber,
		int pageSize,
		string orderBy,
		string orderDirection
	)
	{
		var items = await queryable.ToListAsync();

		return new Page<T>(items, queryableCount, pageNumber, pageSize, orderBy, orderDirection);
	}
	#endregion
}
