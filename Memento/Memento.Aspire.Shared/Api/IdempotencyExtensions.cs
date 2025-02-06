namespace Memento.Aspire.Shared.Api;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;

/// <summary>
/// Implements several extension methods.
/// </summary>
public static class IdempotencyExtensions
{
	#region [Constants]
	/// <summary>
	/// The header name.
	/// </summary>
	public const string HeaderName = "X-Idempotency-Id";
	#endregion

	#region [Extensions]
	/// <summary>
	/// Gets the idempotency identifier or null if it is not valid.
	/// </summary>
	///
	/// <param name="httpContext">The http context.</param>
	public static Guid GetIdempotencyId(this HttpContext httpContext)
	{
		// Check if the header name is valid
		if (!httpContext.Request.Headers.TryGetValue(HeaderName, out var headerValue))
		{
			throw new ArgumentException($"The {HeaderName} header is invalid.");
		}

		// Check if the header value is valid
		if (!Guid.TryParse(headerValue, out var idempotencyId))
		{
			throw new ArgumentException($"The {HeaderName} header is invalid.");
		}

		return idempotencyId;
	}

	/// <summary>
	/// Sets a default idempotency identifier.
	/// </summary>
	///
	/// <param name="httpContext">The http context.</param>
	public static void SetDefaultIdempotencyId(this HttpContext httpContext)
	{
		// Create the idempotency identifier
		var idempotencyId = Guid.NewGuid().ToString();

		// Set the idempotency identifier
		httpContext.Request.Headers.Append(HeaderName, idempotencyId);
	}

	/// <summary>
	/// Tries to get the idempotency identifier.
	/// </summary>
	///
	/// <param name="httpContext">The http context.</param>
	/// <param name="idempotencyId">The idempotency identifier.</param>
	public static bool TryGetIdempotencyId(this HttpContext httpContext, out Guid? idempotencyId)
	{
		idempotencyId = null;

		// Check if the header name is valid
		if (!httpContext.Request.Headers.TryGetValue(HeaderName, out var headerValue))
		{
			return false;
		}

		// Check if the header value is valid
		if (!Guid.TryParse(headerValue, out var innerIdempotencyId))
		{
			return false;
		}

		idempotencyId = innerIdempotencyId;

		return true;
	}

	/// <summary>
	/// Adds the <see cref="IdempotencyEndpointFilter"/> endpoint filter while registed the <see cref="IdempotentAttribute"/> in the endpoints metadata.
	/// </summary>
	///
	/// <param name="endpointConventionBuilder">The endpoint convention builder.</param>
	public static RouteHandlerBuilder RequireIdempotency(this RouteHandlerBuilder endpointConventionBuilder)
	{
		endpointConventionBuilder.Add((convention) =>
		{
			convention.Metadata.Add(new IdempotentAttribute());
		});
		endpointConventionBuilder.AddEndpointFilter<IdempotencyEndpointFilter>();

		return endpointConventionBuilder;
	}
	#endregion
}
