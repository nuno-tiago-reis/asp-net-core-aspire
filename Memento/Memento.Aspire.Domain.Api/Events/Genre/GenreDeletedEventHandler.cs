namespace Memento.Aspire.Domain.Api.Events.Genre;

using Memento.Aspire.Domain.Api.Constants;
using Memento.Aspire.Domain.Service.Messaging.Genre.Events;
using Memento.Aspire.Core.Cache;
using Memento.Aspire.Core.Messaging.Events;
using System.Threading;

/// <summary>
/// Implements the interface for the genre deleted event handler.
/// </summary>
///
/// <seealso cref="EventHandler{T}" />
public sealed class GenreDeletedEventHandler : EventHandler<GenreDeletedEvent>
{
	#region [Properties]
	/// <summary>
	/// The cache.
	/// </summary>
	private readonly ICache Cache;
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="GenreDeletedEventHandler"/> class.
	/// </summary>
	///
	/// <param name="cache">The cache.</param>
	/// <param name="logger">The logger.</param>
	public GenreDeletedEventHandler(ICache cache, ILogger<GenreDeletedEventHandler> logger) : base(logger)
	{
		this.Cache = cache;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	protected override async Task HandleEventAsync(GenreDeletedEvent @event, CancellationToken cancellationToken = default)
	{
		// Remove the genre from the cache
		await this.Cache.RemoveAsync(CacheEntries.GetGenreCacheKey(@event.Genre.Id), cancellationToken);
	}
	#endregion
}
