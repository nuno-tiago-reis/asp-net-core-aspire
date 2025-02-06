namespace Memento.Aspire.Shared.Persistence;

/// <summary>
/// Implements the generic interface for an entity filter.
/// Provides properties to filter the entity queries.
/// </summary>
///
/// <typeparam name="TEntityFilterOrderBy">The entity filter order by type.</typeparam>
/// <typeparam name="TEntityFilterOrderDirection">The entity filter order direction type.</typeparam>
public abstract record EntityFilter<TEntityFilterOrderBy, TEntityFilterOrderDirection> : IEntityFilter<TEntityFilterOrderBy, TEntityFilterOrderDirection>
	where TEntityFilterOrderBy : struct, Enum
	where TEntityFilterOrderDirection : struct, Enum
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the page number.
	/// </summary>
	public required int PageNumber { get; init; }

	/// <summary>
	/// Gets or sets the page size.
	/// </summary>
	public required int PageSize { get; init; }

	/// <summary>
	/// Gets or sets the order by Value.
	/// </summary>
	public required TEntityFilterOrderBy OrderBy { get; init; }

	/// <summary>
	/// Gets or sets the order direction Value.
	/// </summary>
	public required TEntityFilterOrderDirection OrderDirection { get; init; }
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="EntityFilter{TEntityFilterOrder, TEntityFilterOrderDirection}"/> class.
	/// </summary>
	protected EntityFilter()
	{
		this.PageNumber = default;
		this.PageSize = default;
		this.OrderBy = default!;
		this.OrderDirection = default!;
	}
	#endregion
}
