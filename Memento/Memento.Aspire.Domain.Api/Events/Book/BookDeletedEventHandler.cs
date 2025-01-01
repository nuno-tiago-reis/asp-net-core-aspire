namespace Memento.Aspire.Domain.Api.Events.Book;

using Memento.Aspire.Domain.Api.Constants;
using Memento.Aspire.Domain.Service.Messaging.Book.Events;
using Memento.Aspire.Shared.Cache;
using Memento.Aspire.Shared.Messaging.Events;
using System.Threading;

/// <summary>
/// Implements the interface for the book deleted event handler.
/// </summary>
///
/// <seealso cref="EventHandler{T}" />
public sealed class BookDeletedEventHandler : EventHandler<BookDeletedEvent>
{
	#region [Properties]
	/// <summary>
	/// The cache.
	/// </summary>
	private readonly ICache Cache;
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="BookDeletedEventHandler"/> class.
	/// </summary>
	///
	/// <param name="cache">The cache.</param>
	/// <param name="logger">The logger.</param>
	public BookDeletedEventHandler(ICache cache, ILogger<BookDeletedEventHandler> logger) : base(logger)
	{
		this.Cache = cache;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	protected override async Task HandleEventAsync(BookDeletedEvent @event, CancellationToken cancellationToken = default)
	{
		// Remove the book from the cache
		await this.Cache.RemoveAsync(CacheEntries.GetBookCacheKey(@event.BookId), cancellationToken);
	}
	#endregion
}
