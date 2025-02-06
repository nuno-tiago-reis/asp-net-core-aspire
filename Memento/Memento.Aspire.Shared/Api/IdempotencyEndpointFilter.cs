namespace Memento.Aspire.Shared.Api;

using Memento.Aspire.Shared.Cache;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Implements an <seealso cref="IEndpointFilter" /> that augments a method to become idempotent when an identifier is provided.
/// </summary>
public sealed class IdempotencyEndpointFilter : IEndpointFilter
{
	#region [Constants]
	/// <summary>
	/// The cache key.
	/// </summary>
	private static readonly CompositeFormat CacheKey = CompositeFormat.Parse("Idempotency:{0}");
	#endregion

	#region [Methods]
	/// <summary>
	/// Checks if a header is present with a valid IdempotencyKey.
	/// If it is, it attempts to retrieve a cached response and returns the cached response when it exists.
	/// Otherwise it executes the request as normal.
	/// </summary>
	///
	/// <param name="context">The context.</param>
	/// <param name="next">The delegate.</param>
	public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		// Get the dependencies
		var cache = context.HttpContext.RequestServices.GetService<ICache>()!;
		var logger = context.HttpContext.RequestServices.GetService<ILogger<IdempotencyEndpointFilter>>()!;
		var cancellationToken = context.HttpContext.RequestAborted;

		// Check if the idempotency id is valid
		if (!context.HttpContext.TryGetIdempotencyId(out var idempotencyId))
		{
			return await HandleMissingHeaderAsync(context, next, logger);
		}

		// Check if the result is cached
		var cacheKey = string.Format(null, CacheKey, idempotencyId);
		var cacheValue = await cache.TryGetAsync<IdempotencyResult>(cacheKey, cancellationToken);

		if (cacheValue is not null)
		{
			// Log the outcome
			logger.LogInformation("Request has a cached result for IdempotencyId {IdempotencyId}", idempotencyId);

			// Return the cached result
			if (cacheValue.Location is not null)
			{
				return TypedResults.Created(cacheValue.Location, cacheValue.Value);
			}
			else
			{
				return TypedResults.Ok(cacheValue.Value);
			}
		}
		else
		{
			// Log the outcome
			logger.LogInformation("Request does not have a cached result for IdempotencyId {IdempotencyId}", idempotencyId);

			// Execute the request
			var response = await next(context);

			// Cache the result
			if (response is not null && typeof(Created<>) == response.GetType().GetGenericTypeDefinition())
			{
				cacheValue = new IdempotencyResult
				{
					StatusCode = ((IStatusCodeHttpResult)response).StatusCode!.Value,
					Location = ((dynamic)response).Location,
					Value = ((IValueHttpResult)response).Value
				};

				await CacheResultAsync(cache, cacheKey, cacheValue, cancellationToken);
			}
			else if (response is not null && typeof(Ok<>) == response.GetType().GetGenericTypeDefinition())
			{
				cacheValue = new IdempotencyResult
				{
					StatusCode = ((IStatusCodeHttpResult)response).StatusCode!.Value,
					Location = null,
					Value = ((IValueHttpResult)response).Value
				};

				await CacheResultAsync(cache, cacheKey, cacheValue, cancellationToken);
			}

			return response;
		}
	}

	/// <summary>
	/// Bypasses the cache header due to the header being missing.
	/// </summary>
	///
	/// <param name="context">The context.</param>
	/// <param name="next">The delegate.</param>
	/// <param name="logger">The logger.</param>
	private static async ValueTask<object?> HandleMissingHeaderAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next, ILogger logger)
	{
		// Log the outcome
		logger.LogWarning("Request does not have a valid IdempotencyId.");

		// Execute the request
		return await next(context);
	}

	/// <summary>
	/// Caches the given key/value pair.
	/// </summary>
	///
	/// <param name="cache">The cache.</param>
	/// <param name="cacheKey">The cache key.</param>
	/// <param name="cacheValue">The cache value.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	private static async Task CacheResultAsync(ICache cache, string cacheKey, IdempotencyResult cacheValue, CancellationToken cancellationToken)
	{
		// Define the duration
		var absoluteDuration = TimeSpan.FromMinutes(5);
		var slidingDuration = TimeSpan.FromMinutes(5);

		// Store the result in the cache
		await cache.SetAsync(cacheKey, cacheValue, absoluteDuration, slidingDuration, cancellationToken);
	}
	#endregion

	/// <summary>
	/// Implements the idempotency result to be stored in the cache.
	/// </summary>
	private sealed record IdempotencyResult
	{
		/// <summary>
		/// The status code.
		/// </summary>
		public required int StatusCode { get; init; }

		/// <summary>
		/// The location.
		/// </summary>
		public required string? Location { get; init; }

		/// <summary>
		/// The value.
		/// </summary>
		public required object? Value { get; init; }
	}
}
