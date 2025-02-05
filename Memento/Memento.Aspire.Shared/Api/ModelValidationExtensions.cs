namespace Memento.Aspire.Shared.Api;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Mime;

/// <summary>
/// Implements the necessary methods to add the ModelValidation middleware to the ASP.NET Core Pipeline.
/// </summary>
public static class ModelValidationExtensions
{
	#region [Extensions]
	/// <summary>
	/// Customizes the ModelValidation response in the pipeline of the specified <seealso cref="IMvcBuilder"/>.
	/// </summary>
	///
	/// <param name="builder">The builder</param>
	public static IMvcBuilder AddModelValidationProcessing(this IMvcBuilder builder)
	{
		return builder.ConfigureApiBehaviorOptions((behaviorOptions) =>
		{
			behaviorOptions.InvalidModelStateResponseFactory = (context) =>
			{
				// Get the dependencies
				var logger = context.HttpContext.RequestServices.GetService<ILogger<IMvcBuilder>>()!;

				// Log the original error
				logger.LogError("The request is invalid.");

				// Create the standardised error
				var result = new StandardResult
				{
					Success = false,
					Message = "The request is invalid.",
					StatusCode = StatusCodes.Status400BadRequest,
					Errors = ["The request is invalid."]
				};

				// Return the standardised error
				return new ObjectResult(result)
				{
					StatusCode = StatusCodes.Status400BadRequest
				};
			};
		});
	}
	#endregion
}
