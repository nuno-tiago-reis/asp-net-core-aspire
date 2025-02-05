namespace Memento.Aspire.Shared.Api;

using AutoMapper;

/// <summary>
/// Implements the <see cref="ParameterBinder{TSource, TDestination}"/> automapper converter.
/// </summary>
public sealed class ParameterBinderAutoMapperConverter<TSource, TDestination> :
	ITypeConverter<ParameterBinder<TSource>, TDestination>,
	ITypeConverter<TSource, ParameterBinder<TDestination>>
	where TSource : Enum
	where TDestination : Enum
{
	#region [Methods]
	/// <inheritdoc />
	public TDestination Convert(ParameterBinder<TSource> source, TDestination destination, ResolutionContext context)
	{
		return (TDestination)Enum.Parse(typeof(TDestination), source.ToString()!);
	}

	/// <inheritdoc />
	public ParameterBinder<TDestination> Convert(TSource source, ParameterBinder<TDestination> destination, ResolutionContext context)
	{
		return new ParameterBinder<TDestination>
		{
			Value = (TDestination)Enum.Parse(typeof(TDestination), source.ToString()!)
		};
	}
	#endregion
}
