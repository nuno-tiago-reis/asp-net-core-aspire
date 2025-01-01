namespace Memento.Aspire.Domain.Service.Persistence.Entities.Author;

using Memento.Aspire.Shared.Persistence;

/// <summary>
/// Defines the interface for a 'Author' repository.
/// Provides methods to interact with the authors (CRUD and more).
/// </summary>
///
/// <seealso cref="Author" />
/// <seealso cref="AuthorFilter" />
/// <seealso cref="AuthorOrderBy" />
/// <seealso cref="AuthorOrderDirection" />
public interface IAuthorRepository : IEntityRepository<Author, AuthorFilter, AuthorOrderBy, AuthorOrderDirection>
{
	// Intentionally Empty.
}
