namespace Memento.Aspire.Domain.Service.Persistence.Entities.Book;

using Memento.Aspire.Core.Persistence;

/// <summary>
/// Defines the interface for a 'Book' repository.
/// Provides methods to interact with the books (CRUD and more).
/// </summary>
///
/// <seealso cref="Book" />
/// <seealso cref="BookFilter" />
/// <seealso cref="BookOrderBy" />
/// <seealso cref="BookOrderDirection" />
public interface IBookRepository : IEntityRepository<Book, BookFilter, BookOrderBy, BookOrderDirection>
{
	// Intentionally Empty.
}
