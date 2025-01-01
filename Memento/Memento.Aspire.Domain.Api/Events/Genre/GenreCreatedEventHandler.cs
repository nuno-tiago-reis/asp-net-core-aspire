namespace Memento.Aspire.Domain.Api.Events.Genre;

using Memento.Aspire.Domain.Api.Constants;
using Memento.Aspire.Domain.Service.Messaging.Genre.Events;
using Memento.Aspire.Shared.Cache;
using Memento.Aspire.Shared.Messaging.Events;
using System.Threading;

/// <summary>
/// Implements the interface for the genre created event handler.
/// </summary>
///
/// <seealso cref="EventHandler{T}" />
public sealed class GenreCreatedEventHandler : EventHandler<GenreCreatedEvent>
{
	#region [Properties]
	/// <summary>
	/// The cache.
	/// </summary>
	private readonly ICache Cache;
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="GenreCreatedEventHandler"/> class.
	/// </summary>
	///
	/// <param name="cache">The cache.</param>
	/// <param name="logger">The logger.</param>
	public GenreCreatedEventHandler(ICache cache, ILogger<GenreCreatedEventHandler> logger) : base(logger)
	{
		this.Cache = cache;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	protected override async Task HandleEventAsync(GenreCreatedEvent @event, CancellationToken cancellationToken = default)
	{
		// Define the duration
		var absoluteDuration = TimeSpan.FromMinutes(5);
		var slidingDuration = TimeSpan.FromMinutes(5);

		// Store the genre in the cache
		await this.Cache.SetAsync(CacheEntries.GetGenreCacheKey(@event.Genre.Id), @event.Genre, absoluteDuration, slidingDuration, cancellationToken);
	}
	#endregion
}
