namespace Memento.Aspire.Domain.Api.Events.Book;

using Memento.Aspire.Domain.Api.Constants;
using Memento.Aspire.Domain.Service.Messaging.Book.Events;
using Memento.Aspire.Shared.Cache;
using Memento.Aspire.Shared.Messaging.Events;
using System.Threading;

/// <summary>
/// Implements the interface for the book updated event handler.
/// </summary>
///
/// <seealso cref="EventHandler{T}" />
public sealed class BookUpdatedEventHandler : EventHandler<BookUpdatedEvent>
{
	#region [Properties]
	/// <summary>
	/// The cache.
	/// </summary>
	private readonly ICache Cache;
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="BookUpdatedEventHandler"/> class.
	/// </summary>
	///
	/// <param name="cache">The cache.</param>
	/// <param name="logger">The logger.</param>
	public BookUpdatedEventHandler(ICache cache, ILogger<BookUpdatedEventHandler> logger) : base(logger)
	{
		this.Cache = cache;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	protected override async Task HandleEventAsync(BookUpdatedEvent @event, CancellationToken cancellationToken = default)
	{
		// Define the duration
		var absoluteDuration = TimeSpan.FromMinutes(5);
		var slidingDuration = TimeSpan.FromMinutes(5);

		// Store the book in the cache
		await this.Cache.SetAsync(CacheEntries.GetBookCacheKey(@event.Book.Id), @event.Book, absoluteDuration, slidingDuration, cancellationToken);
	}
	#endregion
}
