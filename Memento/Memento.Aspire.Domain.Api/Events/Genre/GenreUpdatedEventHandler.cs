namespace Memento.Aspire.Domain.Api.Events.Genre;

using Memento.Aspire.Domain.Api.Constants;
using Memento.Aspire.Domain.Service.Messaging.Genre.Events;
using Memento.Aspire.Shared.Cache;
using Memento.Aspire.Shared.Messaging.Events;
using System.Threading;

/// <summary>
/// Implements the interface for the genre updated event handler.
/// </summary>
///
/// <seealso cref="EventHandler{T}" />
public sealed class GenreUpdatedEventHandler : EventHandler<GenreUpdatedEvent>
{
	#region [Properties]
	/// <summary>
	/// The cache.
	/// </summary>
	private readonly ICache Cache;
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="GenreUpdatedEventHandler"/> class.
	/// </summary>
	///
	/// <param name="cache">The cache.</param>
	/// <param name="logger">The logger.</param>
	public GenreUpdatedEventHandler(ICache cache, ILogger<GenreUpdatedEventHandler> logger) : base(logger)
	{
		this.Cache = cache;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	protected override async Task HandleEventAsync(GenreUpdatedEvent @event, CancellationToken cancellationToken = default)
	{
		// Store the genre in the cache
		await this.Cache.SetAsync(CacheEntries.GetGenreCacheKey(@event.Genre.Id), @event.Genre, cancellationToken: cancellationToken);
	}
	#endregion
}
