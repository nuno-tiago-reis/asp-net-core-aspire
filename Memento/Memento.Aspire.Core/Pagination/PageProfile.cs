namespace Memento.Aspire.Core.Pagination;

using AutoMapper;

/// <summary>
/// Implements the 'Page' automapper profile.
/// </summary>
///
/// <seealso cref="Profile" />
public sealed class PageProfile : Profile
{
	#region [Constructor]
	/// <summary>
	/// Initializes a new instance of the <see cref="PageProfile"/> class.
	/// </summary>
	public PageProfile()
	{
		this.CreateMap(typeof(Page<>), typeof(Page<>)).ConvertUsing(typeof(PageAutoMapperConverter<,>));
		this.CreateMap(typeof(Page<>), typeof(IPage<>)).ConvertUsing(typeof(PageAutoMapperConverter<,>));
		this.CreateMap(typeof(IPage<>), typeof(Page<>)).ConvertUsing(typeof(PageAutoMapperConverter<,>));
		this.CreateMap(typeof(IPage<>), typeof(IPage<>)).ConvertUsing(typeof(PageAutoMapperConverter<,>));
	}
	#endregion
}
