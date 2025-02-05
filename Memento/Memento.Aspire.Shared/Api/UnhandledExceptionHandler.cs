namespace Memento.Aspire.Shared.Api;

using Memento.Aspire.Shared.Contracts;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Mime;
using System.Text.Json;

/// <summary>
/// Implements the unhandled exception handler.
/// </summary>
///
/// <seealso cref="IExceptionHandler" />
public sealed class UnhandledExceptionHandler : IExceptionHandler
{
	#region [Properties]
	/// <summary>
	/// The logger.
	/// </summary>
	private readonly ILogger Logger;

	/// <summary>
	/// The json options.
	/// </summary>
	private readonly JsonOptions JsonOptions;
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="UnhandledExceptionHandler"/> class.
	/// </summary>
	///
	/// <param name="logger">The logger.</param>
	/// <param name="jsonOptions">The json options.</param>
	public UnhandledExceptionHandler(ILogger<UnhandledExceptionHandler> logger, IOptions<JsonOptions> jsonOptions)
	{
		this.Logger = logger;
		this.JsonOptions = jsonOptions.Value;
	}
	#endregion

	#region [Methods]
	/// <summary>
	/// Returns a standardised result message.
	/// </summary>
	///
	/// <param name="httpContext">The http context.</param>
	/// <param name="exception">The exception.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
	{
		// Log the original error
		this.Logger.LogError("Unhandled Exception: {Message} {Exception}", exception.Message, exception);

		// Create the standardised error
		var result = new StandardResult
		{
			Success = false,
			Message = "An unexpected error has occurred.",
			StatusCode = StatusCodes.Status500InternalServerError,
			Errors = [ "An unexpected error has occurred." ]
		};

		// Write the standardised error
		httpContext.Response.ContentType = MediaTypeNames.Application.Json;
		httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

		await httpContext.Response.WriteAsync(JsonSerializer.Serialize(result, this.JsonOptions.JsonSerializerOptions), cancellationToken);

		return true;
	}
	#endregion
}
