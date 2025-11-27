namespace Memento.Aspire.Domain.Service.Persistence.Entities.Genre;

using Memento.Aspire.Core.Persistence;

/// <summary>
/// Defines the interface for a 'Genre' repository.
/// Provides methods to interact with the genres (CRUD and more).
/// </summary>
///
/// <seealso cref="Genre" />
/// <seealso cref="GenreFilter" />
/// <seealso cref="GenreOrderBy" />
/// <seealso cref="GenreOrderDirection" />
public interface IGenreRepository : IEntityRepository<Genre, GenreFilter, GenreOrderBy, GenreOrderDirection>
{
	// Intentionally Empty.
}
