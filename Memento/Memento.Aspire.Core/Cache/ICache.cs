namespace Memento.Aspire.Core.Cache;

/// <summary>
/// Defines a generic interface for a cache.
/// Provides methods to obtain and store values (indexed by keys) in a cache.
/// </summary>
public interface ICache
{
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
	Task SetAsync<T>(string key, T value, TimeSpan ? absoluteDuration = default, TimeSpan? slidingDuration = default, CancellationToken cancellationToken = default) where T : class;

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
	Task TrySetAsync<T>(string key, T value, TimeSpan? absoluteDuration = default, TimeSpan? slidingDuration = default, CancellationToken cancellationToken = default) where T : class;

	/// <summary>
	/// Gets a Value with the given key.
	/// </summary>
	///
	/// <param name="key">The key.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

	/// <summary>
	/// Gets a Value with the given key.
	/// Will not throw an exception if any error occurs.
	/// </summary>
	///
	/// <param name="key">The key.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	Task<T?> TryGetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

	/// <summary>
	/// Removes a Value with the given key.
	/// </summary>
	///
	/// <param name="key">The key.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	Task RemoveAsync(string key, CancellationToken cancellationToken = default);

	/// <summary>
	/// Removes a Value with the given key.
	/// Will not throw an exception if any error occurs.
	/// </summary>
	///
	/// <param name="key">The key.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	Task TryRemoveAsync(string key, CancellationToken cancellationToken = default);

	/// <summary>
	/// Refreshes a keys sliding duration.
	/// </summary>
	///
	/// <param name="key">The key.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	Task RefreshAsync(string key, CancellationToken cancellationToken = default);

	/// <summary>
	/// Refreshes a keys sliding duration.
	/// Will not throw an exception if any error occurs.
	/// </summary>
	///
	/// <param name="key">The key.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	Task TryRefreshAsync(string key, CancellationToken cancellationToken = default);
	#endregion
}
