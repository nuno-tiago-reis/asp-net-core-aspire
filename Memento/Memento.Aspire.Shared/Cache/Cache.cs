namespace Memento.Aspire.Shared.Cache;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

/// <summary>
/// Implements the generic interface for a cache.
/// Provides methods to obtain and store values (indexed by keys) in a cache.
/// </summary>
public sealed class Cache : ICache
{
	#region [Properties]
	/// <summary>
	/// The distributed cache.
	/// </summary>
	private readonly IDistributedCache DistributedCache;

	/// <summary>
	/// The logger.
	/// </summary>
	private readonly ILogger Logger;

	/// <summary>
	/// The json options.
	/// </summary>
	private readonly JsonOptions JsonOptions;

	/// <summary>
	/// The default absolute duration.
	/// </summary>
	private readonly static TimeSpan DefaultAbsoluteDuration = TimeSpan.FromMinutes(10);

	/// <summary>
	/// The default sliding duration.
	/// </summary>
	private readonly static TimeSpan DefaultSlidingDuration = TimeSpan.FromMinutes(5);
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="Cache"/> class.
	/// </summary>
	///
	/// <param name="distributedCache">The distributed cache.</param>
	/// <param name="logger">The logger.</param>
	/// <param name="jsonOptions">The json options.</param>
	public Cache(IDistributedCache distributedCache, ILogger<Cache> logger, IOptions<JsonOptions> jsonOptions)
	{
		this.DistributedCache = distributedCache;
		this.Logger = logger;
		this.JsonOptions = jsonOptions.Value;
	}
	#endregion

	#region [Methods]
	/// <summary>
	/// Sets a Value with the given key.
	/// The key has an absolute duration after which it will be deleted from the cache.
	/// If the Value isn't accessed for the sliding duration it will be deleted from the cache.
	/// </summary>
	///
	/// <param name="key">The key.</param>
	/// <param name="value">The Value.</param>
	/// <param name="absoluteDuration">The absolute duration.</param>
	/// <param name="slidingDuration">The sliding duration.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteDuration = default, TimeSpan? slidingDuration = default, CancellationToken cancellationToken = default) where T : class
	{
		// Set the default cache options
		var options = new DistributedCacheEntryOptions
		{
			AbsoluteExpirationRelativeToNow = absoluteDuration ?? DefaultAbsoluteDuration,
			SlidingExpiration = slidingDuration ?? DefaultSlidingDuration
		};

		// Convert the object to a byte array
		var bytes = JsonSerializer.SerializeToUtf8Bytes(value, this.JsonOptions.JsonSerializerOptions);

		// Set the byte array
		await this.DistributedCache.SetAsync(key, bytes, options, cancellationToken);
	}

	/// <summary>
	/// Sets a Value with the given key.
	/// The key has an absolute duration after which it will be deleted from the cache.
	/// If the Value isn't accessed for the sliding duration it will be deleted from the cache.
	/// Will not throw an exception if any error occurs.
	/// </summary>
	///
	/// <param name="key">The key.</param>
	/// <param name="value">The Value.</param>
	/// <param name="absoluteDuration">The absolute duration.</param>
	/// <param name="slidingDuration">The sliding duration.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public async Task TrySetAsync<T>(string key, T value, TimeSpan? absoluteDuration = null, TimeSpan? slidingDuration = null, CancellationToken cancellationToken = default) where T : class
	{
		try
		{
			await this.SetAsync(key, value, absoluteDuration, slidingDuration, cancellationToken);
		}
		catch (Exception exception)
		{
			this.Logger.LogError("{Message} {Exception}", exception.Message, exception);
		}
	}

	/// <summary>
	/// Gets a Value with the given key.
	/// </summary>
	///
	/// <param name="key">The key.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
	{
		// Get the byte array
		var bytes = await this.DistributedCache.GetAsync(key, cancellationToken);

		// Convert the byte array to an object
		return JsonSerializer.Deserialize<T>(bytes, this.JsonOptions.JsonSerializerOptions);
	}

	/// <summary>
	/// Gets a Value with the given key.
	/// Will not throw an exception if any error occurs.
	/// </summary>
	///
	/// <param name="key">The key.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public async Task<T?> TryGetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
	{
		try
		{
			return await this.GetAsync<T>(key, cancellationToken);
		}
		catch (Exception exception)
		{
			this.Logger.LogError("{Message} {Exception}", exception.Message, exception);

			return null;
		}
	}

	/// <summary>
	/// Removes a Value with the given key.
	/// </summary>
	///
	/// <param name="key">The key.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
	{
		await this.DistributedCache.RemoveAsync(key, cancellationToken);
	}

	/// <summary>
	/// Removes a Value with the given key.
	/// Will not throw an exception if any error occurs.
	/// </summary>
	///
	/// <param name="key">The key.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public async Task TryRemoveAsync(string key, CancellationToken cancellationToken = default)
	{
		try
		{
			await this.RemoveAsync(key, cancellationToken);
		}
		catch (Exception exception)
		{
			this.Logger.LogError("{Message} {Exception}", exception.Message, exception);
		}
	}

	/// <summary>
	/// Refreshes a keys sliding duration.
	/// </summary>
	///
	/// <param name="key">The key.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public async Task RefreshAsync(string key, CancellationToken cancellationToken = default)
	{
		await this.DistributedCache.RefreshAsync(key, cancellationToken);
	}

	/// <summary>
	/// Refreshes a keys sliding duration.
	/// Will not throw an exception if any error occurs.
	/// </summary>
	///
	/// <param name="key">The key.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public async Task TryRefreshAsync(string key, CancellationToken cancellationToken = default)
	{
		try
		{
			await this.RefreshAsync(key, cancellationToken);
		}
		catch (Exception exception)
		{
			this.Logger.LogError("{Message} {Exception}", exception.Message, exception);
		}
	}
	#endregion
}
