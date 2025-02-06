namespace Memento.Aspire.Shared.Api;

using Memento.Aspire.Shared.Cache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

/// <summary>
/// Implements an <seealso cref="IAsyncActionFilter" /> that augments a method to become idempotent when an identifier is provided.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class IdempotentAttribute : Attribute, IAsyncActionFilter
{
	#region [Constants]
	/// <summary>
	/// The cache key.
	/// </summary>
	private const string CacheKey = "Idempotency:{0}";
	#endregion

	#region [Methods]
	/// <summary>
	/// Checks if a header is present with a valid IdempotencyKey.
	/// If it is, it attempts to retrieve a cached response and returns the cached response when it exists.
	/// Otherwise it executes the request as normal.
	/// </summary>
	///
	/// <param name="executingContext">The execution context.</param>
	/// <param name="next">The next delegate.</param>
	public async Task OnActionExecutionAsync(ActionExecutingContext executingContext, ActionExecutionDelegate next)
	{
		// Get the dependencies
		var cache = executingContext.HttpContext.RequestServices.GetService<ICache>()!;
		var logger = executingContext.HttpContext.RequestServices.GetService<ILogger<IdempotentAttribute>>()!;
		var cancellationToken = executingContext.HttpContext.RequestAborted;

		// Check if the idempotency id is valid
		if (!executingContext.HttpContext.TryGetIdempotencyId(out var idempotencyId))
		{
			await HandleMissingHeaderAsync(next, logger);
			return;
		}

		// Check if the result is cached
		var cacheKey = string.Format(CacheKey, idempotencyId);
		var cacheValue = await cache.TryGetAsync<IdempotencyResult>(cacheKey, cancellationToken);

		if (cacheValue is not null)
		{
			// Log the outcome
			logger.LogInformation("Request has a cached result for IdempotencyId {IdempotencyId}", idempotencyId);

			// Return the cached result
			if (cacheValue.Location is not null)
			{
				executingContext.Result = new CreatedResult(cacheValue.Location, cacheValue.Value)
				{
					StatusCode = cacheValue.StatusCode
				};
			}
			else
			{
				executingContext.Result = new ObjectResult(cacheValue.Value)
				{
					StatusCode = cacheValue.StatusCode
				};
			}
		}
		else
		{
			// Log the outcome
			logger.LogInformation("Request does not have a cached result for IdempotencyId {IdempotencyId}", idempotencyId);

			// Execute the request
			var executedContext = await next();

			// Cache the result
			if (executedContext.Result is CreatedResult createdResult && createdResult.StatusCode is not null)
			{
				cacheValue = new IdempotencyResult
				{
					StatusCode = createdResult.StatusCode.Value,
					Location = createdResult.Location,
					Value = createdResult.Value
				};

				await CacheResultAsync(cache, cacheKey, cacheValue, cancellationToken);
			}
			else if (executedContext.Result is ObjectResult objectResult && objectResult.StatusCode == 200)
			{
				cacheValue = new IdempotencyResult
				{
					StatusCode = objectResult.StatusCode.Value,
					Location = null,
					Value = objectResult.Value
				};

				await CacheResultAsync(cache, cacheKey, cacheValue, cancellationToken);
			}
		}
	}

	/// <summary>
	/// Bypasses the cache header due to the header being missing.
	/// </summary>
	///
	/// <param name="invocationContext">The invocation context.</param>
	/// <param name="next">The next delegate.</param>
	/// <param name="logger">The logger.</param>
	private static async Task HandleMissingHeaderAsync(ActionExecutionDelegate next, ILogger logger)
	{
		// Log the outcome
		logger.LogWarning("Request does not have a valid IdempotencyId.");

		// Execute the request
		await next();
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
