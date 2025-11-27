namespace Memento.Aspire.Core.Persistence;

/// <summary>
/// Defines the generic interface for an entity filter.
/// Provides properties to filter the entity queries.
/// </summary>
///
/// <typeparam name="TEntityFilterOrderBy">The entity filter order by type.</typeparam>
/// <typeparam name="TEntityFilterOrderDirection">The entity filter order direction type.</typeparam>
public interface IEntityFilter<TEntityFilterOrderBy, TEntityFilterOrderDirection>
	where TEntityFilterOrderBy : struct, Enum
	where TEntityFilterOrderDirection : struct, Enum
{
	#region [Properties]
	/// <summary>
	/// Gets or sets the page number.
	/// </summary>
	int PageNumber { get; init; }

	/// <summary>
	/// Gets or sets the page size.
	/// </summary>
	int PageSize { get; init; }

	/// <summary>
	/// Gets or sets the order by Value.
	/// </summary>
	TEntityFilterOrderBy OrderBy { get; init; }

	/// <summary>
	/// Gets or sets the order direction Value.
	/// </summary>
	TEntityFilterOrderDirection OrderDirection { get; init; }
	#endregion
}
