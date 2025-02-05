namespace Memento.Aspire.Domain.Service.Persistence.Mapping;

using AutoMapper;
using Memento.Aspire.Domain.Service.Contracts.Genre;
using Memento.Aspire.Domain.Service.Persistence.Entities.Genre;
using Memento.Aspire.Shared.Api;

/// <summary>
/// Implements the 'Genre' profile.
/// </summary>
///
/// <seealso cref="Profile" />
public sealed class GenreProfile : Profile
{
	#region [Constructor]
	/// <summary>
	/// Initializes a new instance of the <see cref="GenreProfile"/> class.
	/// </summary>
	public GenreProfile()
	{
		// Entity => Contract
		this.CreateMap<Genre, GenreDetailContract>();
		// Entity => Contract
		this.CreateMap<Genre, GenreFormContract>();
		// Entity => Contract
		this.CreateMap<Genre, GenreSummaryContract>();

		// Contract => Entity
		this.CreateMap<GenreFormContract, Genre>();

		// FilterContract => Filter
		this.CreateMap<GenreFilterContract, GenreFilter>();
		// Filter => FilterContract
		this.CreateMap<GenreFilter, GenreFilterContract>();

		// EnumContract => Enum
		this.CreateMap<Contracts.Genre.GenreOrderBy, Entities.Genre.GenreOrderBy>();
		this.CreateMap<Contracts.Genre.GenreOrderDirection, Entities.Genre.GenreOrderDirection>();
		// Enum => EnumContract
		this.CreateMap<Entities.Genre.GenreOrderBy, Contracts.Genre.GenreOrderBy>();
		this.CreateMap<Entities.Genre.GenreOrderDirection, Contracts.Genre.GenreOrderDirection>();

		// EnumContract => Enum
		this.CreateMap<ParameterBinder<Contracts.Genre.GenreOrderBy>, Entities.Genre.GenreOrderBy>()
			.ConvertUsing<ParameterBinderAutoMapperConverter<Contracts.Genre.GenreOrderBy, Entities.Genre.GenreOrderBy>>();
		this.CreateMap<ParameterBinder<Contracts.Genre.GenreOrderDirection>, Entities.Genre.GenreOrderDirection>()
			.ConvertUsing<ParameterBinderAutoMapperConverter<Contracts.Genre.GenreOrderDirection, Entities.Genre.GenreOrderDirection>>();
		// Enum => EnumContract
		this.CreateMap<Entities.Genre.GenreOrderBy, ParameterBinder<Contracts.Genre.GenreOrderBy>>()
			.ConvertUsing<ParameterBinderAutoMapperConverter<Entities.Genre.GenreOrderBy, Contracts.Genre.GenreOrderBy>>();
		this.CreateMap<Entities.Genre.GenreOrderDirection, ParameterBinder<Contracts.Genre.GenreOrderDirection>>()
			.ConvertUsing<ParameterBinderAutoMapperConverter<Entities.Genre.GenreOrderDirection, Contracts.Genre.GenreOrderDirection>>();
	}
	#endregion
}
