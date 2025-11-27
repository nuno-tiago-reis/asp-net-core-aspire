namespace Memento.Aspire.Domain.Api.Events.Author;

using Memento.Aspire.Domain.Api.Constants;
using Memento.Aspire.Domain.Service.Messaging.Author.Events;
using Memento.Aspire.Core.Cache;
using Memento.Aspire.Core.Messaging.Events;
using System.Threading;

/// <summary>
/// Implements the interface for the author deleted event handler.
/// </summary>
///
/// <seealso cref="EventHandler{T}" />
public sealed class AuthorDeletedEventHandler : EventHandler<AuthorDeletedEvent>
{
	#region [Properties]
	/// <summary>
	/// The cache.
	/// </summary>
	private readonly ICache Cache;
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="AuthorDeletedEventHandler"/> class.
	/// </summary>
	///
	/// <param name="cache">The cache.</param>
	/// <param name="logger">The logger.</param>
	public AuthorDeletedEventHandler(ICache cache, ILogger<AuthorDeletedEventHandler> logger) : base(logger)
	{
		this.Cache = cache;
	}
	#endregion

	#region [Methods]
	/// <inheritdoc />
	protected override async Task HandleEventAsync(AuthorDeletedEvent @event, CancellationToken cancellationToken = default)
	{
		// Remove the author from the cache
		await this.Cache.RemoveAsync(CacheEntries.GetAuthorCacheKey(@event.Author.Id), cancellationToken);
	}
	#endregion
}
