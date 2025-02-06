namespace Memento.Aspire.Domain.Api.Constants;

/// <summary>
/// Implements several utility methods for dealing with cache entries.
/// </summary>
internal static class CacheEntries
{
	#region [Properties]
	/// <summary>
	/// The template for the cache key for Author cache entries.
	/// </summary>
	private static readonly string Author = "Author:{0}";

	/// <summary>
	/// The template for the cache key for Book cache entries.
	/// </summary>
	private static readonly string Book = "Book:{0}";

	/// <summary>
	/// The template for the cache key for Genre cache entries.
	/// </summary>
	private static readonly string Genre = "Genre:{0}";
	#endregion

	#region [Methods]
	/// <summary>
	/// Gets the cache key for an Author cache entry.
	/// </summary>
	///
	/// <param name="authorId"></param>
	public static string GetAuthorCacheKey(Guid authorId)
	{
		return string.Format(Author, authorId);
	}

	/// <summary>
	/// Gets the cache key for an Book cache entry.
	/// </summary>
	///
	/// <param name="bookId"></param>
	public static string GetBookCacheKey(Guid bookId)
	{
		return string.Format(Book, bookId);
	}

	/// <summary>
	/// Gets the cache key for an Genre cache entry.
	/// </summary>
	///
	/// <param name="genreId"></param>
	public static string GetGenreCacheKey(Guid genreId)
	{
		return string.Format(Genre, genreId);
	}
	#endregion
}
