namespace Memento.Aspire.Core.Api;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

/// <summary>
/// Implements an <seealso cref="IAsyncActionFilter" /> that ensures a correlation identifier is present.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class CorrelateAttribute : Attribute, IAsyncActionFilter
{
	#region [Methods]
	/// <summary>
	/// Checks if a header is present with a valid CorrelationKey and sets a default value when it is not present.
	/// </summary>
	///
	/// <param name="context">The context.</param>
	/// <param name="next">The delegate.</param>
	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		// Get the dependencies
		var logger = context.HttpContext.RequestServices.GetService<ILogger<CorrelateAttribute>>()!;

		// Check if the correlation id is valid
		if (!context.HttpContext.TryGetCorrelationId(out var _))
		{
			HandleMissingHeader(context, logger);
		}

		// Execute the request
		await next();
	}

	/// <summary>
	/// Generates a default correlation identifier header due to the header being missing.
	/// </summary>
	///
	/// <param name="context">The context.</param>
	/// <param name="logger">The logger.</param>
	private static void HandleMissingHeader(ActionExecutingContext context, ILogger logger)
	{
		// Log the outcome
		logger.LogWarning("Request does not have a valid CorrelationId.");

		// Generate a default value
		context.HttpContext.SetDefaultCorrelationId();
	}
	#endregion
}
