namespace Memento.Aspire.Domain.Service.Persistence.Mapping;

using AutoMapper;
using Memento.Aspire.Domain.Service.Contracts.Book;
using Memento.Aspire.Domain.Service.Persistence.Entities.Book;

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
	}
	#endregion
}
