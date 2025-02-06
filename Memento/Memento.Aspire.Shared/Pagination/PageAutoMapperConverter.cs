namespace Memento.Aspire.Shared.Pagination;

using AutoMapper;

/// <summary>
/// Implements the <see cref="Page{}"/> automapper converter.
/// </summary>
public sealed class PageAutoMapperConverter<TSource, TDestination> :
	ITypeConverter<IPage<TSource>, IPage<TDestination>>,
	ITypeConverter<IPage<TSource>, Page<TDestination>>,
	ITypeConverter<Page<TSource>, IPage<TDestination>>,
	ITypeConverter<Page<TSource>, Page<TDestination>>
{
	#region [Methods]
	/// <inheritdoc />
	public IPage<TDestination> Convert(IPage<TSource> source, IPage<TDestination> destination, ResolutionContext context)
	{
		return Page<TDestination>.CreateUnmodified
		(
			context.Mapper.Map<TDestination[]>(source.Items),
			source.Items.Length,
			source.PageNumber,
			source.PageSize,
			source.OrderBy!,
			source.OrderDirection!
		);
	}

	/// <inheritdoc />
	public Page<TDestination> Convert(IPage<TSource> source, Page<TDestination> destination, ResolutionContext context)
	{
		return Page<TDestination>.CreateUnmodified
		(
			context.Mapper.Map<TDestination[]>(source.Items),
			source.Items.Length,
			source.PageNumber,
			source.PageSize,
			source.OrderBy!,
			source.OrderDirection!
		);
	}

	/// <inheritdoc />
	public IPage<TDestination> Convert(Page<TSource> source, IPage<TDestination> destination, ResolutionContext context)
	{
		return Page<TDestination>.CreateUnmodified
		(
			context.Mapper.Map<TDestination[]>(source.Items),
			source.Items.Length,
			source.PageNumber,
			source.PageSize,
			source.OrderBy!,
			source.OrderDirection!
		);
	}

	/// <inheritdoc />
	public Page<TDestination> Convert(Page<TSource> source, Page<TDestination> destination, ResolutionContext context)
	{
		return Page<TDestination>.CreateUnmodified
		(
			context.Mapper.Map<TDestination[]>(source.Items),
			source.Items.Length,
			source.PageNumber,
			source.PageSize,
			source.OrderBy!,
			source.OrderDirection!
		);
	}
	#endregion
}
