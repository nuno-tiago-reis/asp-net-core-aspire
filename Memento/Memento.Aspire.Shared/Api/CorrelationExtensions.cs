namespace Memento.Aspire.Shared.Api;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;

/// <summary>
/// Implements several extension methods.
/// </summary>
public static class CorrelationExtensions
{
	#region [Constants]
	/// <summary>
	/// The header name.
	/// </summary>
	public const string HeaderName = "X-Correlation-Id";
	#endregion

	#region [Extensions]
	/// <summary>
	/// Gets the correlation identifier or null if it is not valid.
	/// </summary>
	///
	/// <param name="httpContext">The http context.</param>
	public static Guid GetCorrelationId(this HttpContext httpContext)
	{
		// Check if the header name is valid
		if (!httpContext.Request.Headers.TryGetValue(HeaderName, out StringValues headerValue))
		{
			throw new ArgumentException($"The {HeaderName} header is invalid.");
		}

		// Check if the header value is valid
		if (!Guid.TryParse(headerValue, out Guid correlationId))
		{
			throw new ArgumentException($"The {HeaderName} header is invalid.");
		}

		return correlationId;
	}

	/// <summary>
	/// Tries to get the correlation identifier.
	/// </summary>
	///
	/// <param name="httpContext">The http context.</param>
	/// <param name="correlationId">The correlation identifier.</param>
	public static bool TryGetCorrelationId(this HttpContext httpContext, out Guid? correlationId)
	{
		correlationId = null;

		// Check if the header name is valid
		if (!httpContext.Request.Headers.TryGetValue(HeaderName, out StringValues headerValue))
		{
			return false;
		}

		// Check if the header value is valid
		if (!Guid.TryParse(headerValue, out Guid innerCorrelationId))
		{
			return false;
		}

		correlationId = innerCorrelationId;

		return true;
	}

	/// <summary>
	/// Sets a default correlation identifier.
	/// </summary>
	///
	/// <param name="httpContext">The http context.</param>
	public static void SetDefaultCorrelationId(this HttpContext httpContext)
	{
		// Create the correlation identifier
		var correlationId = Guid.NewGuid().ToString();

		// Set the correlation identifier
		httpContext.Request.Headers.Append(HeaderName, correlationId);
	}

	/// <summary>
	/// Adds the <see cref="CorrelationEndpointFilter"/> endpoint filter while registed the <see cref="CorrelateAttribute"/> in the endpoints metadata.
	/// </summary>
	///
	/// <param name="endpointConventionBuilder">The endpoint convention builder.</param>
	public static RouteHandlerBuilder RequireCorrelation(this RouteHandlerBuilder endpointConventionBuilder)
	{
		endpointConventionBuilder.Add((convention) =>
		{
			convention.Metadata.Add(new CorrelateAttribute());
		});
		endpointConventionBuilder.AddEndpointFilter<CorrelationEndpointFilter>();

		return endpointConventionBuilder;
	}
	#endregion
}
