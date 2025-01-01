namespace Memento.Aspire.Domain.Service.Persistence.Mapping;

using AutoMapper;
using Memento.Aspire.Domain.Service.Contracts.Author;
using Memento.Aspire.Domain.Service.Persistence.Entities.Author;
using Memento.Aspire.Shared.Binding;

/// <summary>
/// Implements the 'Author' profile.
/// </summary>
///
/// <seealso cref="Profile" />
public sealed class AuthorProfile : Profile
{
	#region [Constructor]
	/// <summary>
	/// Initializes a new instance of the <see cref="AuthorProfile"/> class.
	/// </summary>
	public AuthorProfile()
	{
		// Entity => Contract
		this.CreateMap<Author, AuthorDetailContract>();
		// Entity => Contract
		this.CreateMap<Author, AuthorFormContract>();
		// Entity => Contract
		this.CreateMap<Author, AuthorSummaryContract>();

		// Contract => Entity
		this.CreateMap<AuthorFormContract, Author>();

		// FilterContract => Filter
		this.CreateMap<AuthorFilterContract, AuthorFilter>();
		// Filter => FilterContract
		this.CreateMap<AuthorFilter, AuthorFilterContract>();

		// EnumContract => Enum
		this.CreateMap<Contracts.Author.AuthorOrderBy, Entities.Author.AuthorOrderBy>();
		this.CreateMap<Contracts.Author.AuthorOrderDirection, Entities.Author.AuthorOrderDirection>();
		// Enum => EnumContract
		this.CreateMap<Entities.Author.AuthorOrderBy, Contracts.Author.AuthorOrderBy>();
		this.CreateMap<Entities.Author.AuthorOrderDirection, Contracts.Author.AuthorOrderDirection>();

		// EnumContract => Enum
		this.CreateMap<ParameterBinder<Contracts.Author.AuthorOrderBy>, Entities.Author.AuthorOrderBy>()
			.ConvertUsing<ParameterBinderAutoMapperConverter<Contracts.Author.AuthorOrderBy, Entities.Author.AuthorOrderBy>>();
		this.CreateMap<ParameterBinder<Contracts.Author.AuthorOrderDirection>, Entities.Author.AuthorOrderDirection>()
			.ConvertUsing<ParameterBinderAutoMapperConverter<Contracts.Author.AuthorOrderDirection, Entities.Author.AuthorOrderDirection>>();
		// Enum => EnumContract
		this.CreateMap<Entities.Author.AuthorOrderBy, ParameterBinder<Contracts.Author.AuthorOrderBy>>()
			.ConvertUsing<ParameterBinderAutoMapperConverter<Entities.Author.AuthorOrderBy, Contracts.Author.AuthorOrderBy>>();
		this.CreateMap<Entities.Author.AuthorOrderDirection, ParameterBinder<Contracts.Author.AuthorOrderDirection>>()
			.ConvertUsing<ParameterBinderAutoMapperConverter<Entities.Author.AuthorOrderDirection, Contracts.Author.AuthorOrderDirection>>();
	}
	#endregion
}
