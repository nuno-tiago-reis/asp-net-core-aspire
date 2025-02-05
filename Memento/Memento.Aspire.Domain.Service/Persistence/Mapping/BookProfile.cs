namespace Memento.Aspire.Domain.Service.Persistence.Mapping;

using AutoMapper;
using Memento.Aspire.Domain.Service.Contracts.Book;
using Memento.Aspire.Domain.Service.Persistence.Entities.Book;
using Memento.Aspire.Shared.Api;

/// <summary>
/// Implements the 'Book' profile.
/// </summary>
///
/// <seealso cref="Profile" />
public sealed class BookProfile : Profile
{
	#region [Constructor]
	/// <summary>
	/// Initializes a new instance of the <see cref="BookProfile"/> class.
	/// </summary>
	public BookProfile()
	{
		// Entity => Contract
		this.CreateMap<Book, BookDetailContract>();
		// Entity => Contract
		this.CreateMap<Book, BookFormContract>();
		// Entity => Contract
		this.CreateMap<Book, BookSummaryContract>();

		// Contract => Entity
		this.CreateMap<BookFormContract, Book>();

		// FilterContract => Filter
		this.CreateMap<BookFilterContract, BookFilter>();
		// Filter => FilterContract
		this.CreateMap<BookFilter, BookFilterContract>();

		// EnumContract => Enum
		this.CreateMap<Contracts.Book.BookOrderBy, Entities.Book.BookOrderBy>();
		this.CreateMap<Contracts.Book.BookOrderDirection, Entities.Book.BookOrderDirection>();
		// Enum => EnumContract
		this.CreateMap<Entities.Book.BookOrderBy, Contracts.Book.BookOrderBy>();
		this.CreateMap<Entities.Book.BookOrderDirection, Contracts.Book.BookOrderDirection>();

		// EnumContract => Enum
		this.CreateMap<ParameterBinder<Contracts.Book.BookOrderBy>, Entities.Book.BookOrderBy>()
			.ConvertUsing<ParameterBinderAutoMapperConverter<Contracts.Book.BookOrderBy, Entities.Book.BookOrderBy>>();
		this.CreateMap<ParameterBinder<Contracts.Book.BookOrderDirection>, Entities.Book.BookOrderDirection>()
			.ConvertUsing<ParameterBinderAutoMapperConverter<Contracts.Book.BookOrderDirection, Entities.Book.BookOrderDirection>>();
		// Enum => EnumContract
		this.CreateMap<Entities.Book.BookOrderBy, ParameterBinder<Contracts.Book.BookOrderBy>>()
			.ConvertUsing<ParameterBinderAutoMapperConverter<Entities.Book.BookOrderBy, Contracts.Book.BookOrderBy>>();
		this.CreateMap<Entities.Book.BookOrderDirection, ParameterBinder<Contracts.Book.BookOrderDirection>>()
			.ConvertUsing<ParameterBinderAutoMapperConverter<Entities.Book.BookOrderDirection, Contracts.Book.BookOrderDirection>>();
	}
	#endregion
}
