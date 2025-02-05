namespace Memento.Aspire.Shared.Api;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Implements an <seealso cref="IEndpointFilter" /> that ensures a correlation identifier is present.
/// </summary>
public sealed class CorrelationEndpointFilter : IEndpointFilter
{
	#region [Methods]
	/// <summary>
	/// Checks if a header is present with a valid CorrelationKey and sets a default value when it is not present.
	/// </summary>
	///
	/// <param name="invocationContext">The invocation context.</param>
	/// <param name="next">The next delegate.</param>
	public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext invocationContext, EndpointFilterDelegate next)
	{
		// Get the dependencies
		var logger = invocationContext.HttpContext.RequestServices.GetService<ILogger<CorrelationEndpointFilter>>()!;

		// Check if the correlation id is valid
		if (!invocationContext.HttpContext.TryGetCorrelationId(out var _))
		{
			HandleMissingHeader(invocationContext, logger);
		}

		// Execute the request
		return await next(invocationContext);
	}

	/// <summary>
	/// Generates a default correlation identifier header due to the header being missing.
	/// </summary>
	///
	/// <param name="invocationContext">The invocation context.</param>
	/// <param name="logger">The logger.</param>
	private static void HandleMissingHeader(EndpointFilterInvocationContext invocationContext, ILogger logger)
	{
		// Log the outcome
		logger.LogWarning("Request does not have a valid CorrelationId.");

		// Generate a default value
		invocationContext.HttpContext.SetDefaultCorrelationId();
	}
	#endregion
}
