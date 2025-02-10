namespace Memento.Aspire.Domain.Api.Events.Author;

using Memento.Aspire.Domain.Api.Constants;
using Memento.Aspire.Domain.Service.Messaging.Author.Events;
using Memento.Aspire.Shared.Cache;
using Memento.Aspire.Shared.Messaging.Events;
using System.Threading;

/// <summary>
/// Implements the interface for the author updated event handler.
/// </summary>
///
/// <seealso cref="EventHandler{T}" />
public sealed class AuthorUpdatedEventHandler : EventHandler<AuthorUpdatedEvent>
{
	#region [Properties]
	/// <summary>
	/// The cache.
	/// </summary>
	private readonly ICache Cache;
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="AuthorUpdatedEventHandler"/> class.
	/// </summary>
	///
	/// <param name="cache">The cache.</param>
	/// <param name="logger">The logger.</param>
	public AuthorUpdatedEventHandler(ICache cache, ILogger<AuthorUpdatedEventHandler> logger) : base(logger)
	{
		this.Cache = cache;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	protected override async Task HandleEventAsync(AuthorUpdatedEvent @event, CancellationToken cancellationToken = default)
	{
		// Store the author in the cache
		await this.Cache.SetAsync(CacheEntries.GetAuthorCacheKey(@event.Author.Id), @event.Author, cancellationToken: cancellationToken);
	}
	#endregion
}
