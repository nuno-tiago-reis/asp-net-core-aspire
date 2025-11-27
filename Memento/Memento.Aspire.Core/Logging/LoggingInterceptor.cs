namespace Memento.Aspire.Core.Logging;

using Microsoft.AspNetCore.HttpLogging;

/// <summary>
/// Implements an <see cref="IHttpLoggingInterceptor"/> interceptor that excludes swagger requests/responses.
/// </summary>
public sealed class LoggingInterceptor : IHttpLoggingInterceptor
{
	#region [Methods]
	/// <inheritdoc/>
	public ValueTask OnRequestAsync(HttpLoggingInterceptorContext logContext)
	{
		if (logContext.HttpContext.Request.Path.Value?.Contains("swagger", StringComparison.InvariantCultureIgnoreCase) == true)
		{
			logContext.TryDisable(HttpLoggingFields.Request | HttpLoggingFields.Response);
		}

		return default;
	}

	/// <inheritdoc/>
	public ValueTask OnResponseAsync(HttpLoggingInterceptorContext logContext)
	{
		if (logContext.HttpContext.Request.Path.Value?.Contains("swagger", StringComparison.InvariantCultureIgnoreCase) == true)
		{
			logContext.TryDisable(HttpLoggingFields.Request | HttpLoggingFields.Response);
		}

		return default;
	}
	#endregion
}
