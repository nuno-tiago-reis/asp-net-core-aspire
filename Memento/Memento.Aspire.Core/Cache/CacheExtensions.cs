namespace Memento.Aspire.Core.Cache;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Implements the necessary methods to add an implementation of <see cref="ICache" /> to the ASP.NET Core dependencies.
/// </summary>
public static class CacheExtensions
{
	#region [Extensions]
	/// <summary>
	/// Adds an instance of <see cref="ICache"/> to the specified <seealso cref="IServiceCollection"/>.
	/// Uses the specified <seealso cref="CacheOptions"/>
	/// </summary>
	///
	/// <param name="services">The service collection.</param>
	/// <param name="options">The redisOptions.</param>
	public static IServiceCollection AddCache(this IServiceCollection services, CacheOptions options)
	{
		ValidateOptions(options);

		// Register the redisOptions
		services.AddSingleton(options);

		// Register the service
		services.AddScoped<ICache, Cache>();

		// Register the distributed cache
		services.AddStackExchangeRedisCache((redisOptions) =>
		{
			redisOptions.Configuration = options.ConnectionString;
		});

		return services;
	}

	/// <summary>
	/// Adds an instance of <see cref="ICache"/> to the specified <seealso cref="IServiceCollection"/>.
	/// Uses the specified <seealso cref="CacheOptions"/>
	/// </summary>
	///
	/// <param name="services">The service collection.</param>
	/// <param name="action">The action that configures the <seealso cref="CacheOptions"/>.</param>
	public static IServiceCollection AddCache(this IServiceCollection services, Action<CacheOptions> action)
	{
		// Create the redisOptions
		var options = new CacheOptions
		{
			ConnectionString = null
		};

		// Configure the redisOptions
		action?.Invoke(options);

		// Register the service
		services.AddCache(options);

		return services;
	}
	#endregion

	#region [Helpers]
	/// <summary>
	/// Validates the redisOptions.
	/// </summary>
	///
	/// <param name="options">The redisOptions.</param>
	private static void ValidateOptions(CacheOptions options)
	{
		// Validate the redisOptions
		if (options is null)
		{
			throw new ArgumentException($"The {nameof(options)} are invalid.");
		}

		// Validate the host
		if (string.IsNullOrWhiteSpace(options.ConnectionString))
		{
			throw new ArgumentException($"The {nameof(options.ConnectionString)} parameter is invalid.");
		}
	}
	#endregion
}
