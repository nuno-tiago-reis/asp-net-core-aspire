namespace Memento.Aspire.Shared.Api;

using Carter;
using FluentValidation.Results;
using Memento.Aspire.Shared.Contracts;
using Memento.Aspire.Shared.Exceptions;
using Memento.Aspire.Shared.Localization;
using Memento.Aspire.Shared.Pagination;
using Memento.Aspire.Shared.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Implements an abstract module that provides the base for the CRUD operations.
/// </summary>
///
/// <seealso cref="CarterModule" />
public abstract class EntityModule : CarterModule
{
	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="EntityModule"/> class.
	/// </summary>
	///
	/// <param name="route">The route.</param>
	public EntityModule(string route) : base(route)
	{
		// Intentionally Empty.
	}
	#endregion

	#region [Methods] Module
	/// <summary>
	/// Adds the configured minimal API routes.
	/// </summary>
	///
	/// <param name="builder">The builder.</param>
	public override abstract void AddRoutes(IEndpointRouteBuilder builder);
	#endregion

	#region [Methods] Results
	/// <summary>
	/// Builds an <seealso cref="IResult"/> response for a successful 'Create' operation.
	/// </summary>
	///
	/// <typeparam name="TContract">The contract type.</typeparam>
	///
	/// <param name="httpContext">The http context.</param>
	/// <param name="contract">The contract.</param>
	protected Created<StandardResult<TContract>> BuildCreateResult<TContract>(HttpContext httpContext, TContract contract) where TContract : EntityContract
	{
		// Get the services
		var localizer = httpContext.RequestServices.GetService<ILocalizer>()!;

		// Build the message
		var message = this.GetCreateSuccessfulMessage(localizer);

		// Build the response
		var response = new StandardResult<TContract>
		{
			Success = true,
			Message = message,
			StatusCode = StatusCodes.Status201Created,
			Data = contract,
			Errors = []
		};

		return TypedResults.Created(new Uri($"{httpContext.Request.GetDisplayUrl()}/{contract.Id}"), response);
	}

	/// <summary>
	/// Builds an <seealso cref="IResult"/> response for an successful 'Update' operation.
	/// </summary>
	///
	/// <param name="httpContext">The http context.</param>
	protected Ok<StandardResult> BuildUpdateResult(HttpContext httpContext)
	{
		// Get the services
		var localizer = httpContext.RequestServices.GetService<ILocalizer>()!;

		// Build the message
		var message = this.GetUpdateSuccessfulMessage(localizer);

		// Build the response
		var response = new StandardResult
		{
			Success = true,
			Message = message,
			StatusCode = StatusCodes.Status200OK,
			Errors = []
		};

		return TypedResults.Ok(response);
	}

	/// <summary>
	/// Builds an <seealso cref="IResult"/> response for a successful 'Delete' operation.
	/// </summary>
	///
	/// <param name="httpContext">The http context.</param>
	protected Ok<StandardResult> BuildDeleteResult(HttpContext httpContext)
	{
		// Get the services
		var localizer = httpContext.RequestServices.GetService<ILocalizer>()!;

		// Build the message
		var message = this.GetDeleteSuccessfulMessage(localizer);

		// Build the response
		var response = new StandardResult
		{
			Success = true,
			Message = message,
			StatusCode = StatusCodes.Status200OK,
			Errors = []
		};

		return TypedResults.Ok(response);
	}

	/// <summary>
	/// Builds an <seealso cref="IResult"/> response for a successful 'Get' operation.
	/// </summary>
	///
	/// <typeparam name="TContract">The contract type.</typeparam>
	///
	/// <param name="contract">The contract.</param>
	protected Ok<StandardResult<TContract>> BuildGetResult<TContract>(HttpContext httpContext, TContract contract) where TContract : EntityContract
	{
		// Get the services
		var localizer = httpContext.RequestServices.GetService<ILocalizer>()!;

		// Build the message
		var message = this.GetGetSuccessfulMessage(localizer);

		// Build the response
		var response = new StandardResult<TContract>
		{
			Success = true,
			Message = message,
			StatusCode = StatusCodes.Status200OK,
			Data = contract,
			Errors = []
		};

		return TypedResults.Ok(response);
	}

	/// <summary>
	/// Builds an <seealso cref="IResult"/> response for a successful 'GetAll' operation.
	/// </summary>
	///
	/// <typeparam name="TContract">The contract type.</typeparam>
	///
	/// <param name="contracts">The contracts.</param>
	protected Ok<StandardResult<Page<TContract>>> BuildGetAllResult<TContract>(HttpContext httpContext, Page<TContract> contracts) where TContract : EntityContract
	{
		// Get the services
		var localizer = httpContext.RequestServices.GetService<ILocalizer>()!;

		// Build the message
		var message = this.GetGetAllSuccessfulMessage(localizer);

		// Build the response
		var response = new StandardResult<Page<TContract>>
		{
			Success = true,
			Message = message,
			StatusCode = StatusCodes.Status200OK,
			Data = contracts,
			Errors = []
		};

		return TypedResults.Ok(response);
	}

	/// <summary>
	/// Builds an <seealso cref="IResult"/> response for an unsuccessful operation.
	/// </summary>
	///
	/// <param name="httpContext">The http context.</param>
	protected IResult BuildErrorResult(HttpContext httpContext, StandardException exception)
	{
		// Get the services
		var localizer = httpContext.RequestServices.GetService<ILocalizer>()!;

		// Build the response
		var response = new StandardResult
		{
			Success = false,
			Message = exception.Message,
			StatusCode = exception.GetStatusCode(),
			Errors = exception.Messages
		};

		// Adjust the message when necessary
		switch (response.StatusCode)
		{
			case StatusCodes.Status400BadRequest:
			{
				response.Message = this.GetValidationErrorMessage(localizer);

				return TypedResults.BadRequest(response);
			}
			case StatusCodes.Status404NotFound:
			{
				return TypedResults.NotFound(response);
			}
			default:
			{
				response.Message = this.GetUnexpectedErrorMessage(localizer);
				response.Errors = [response.Message];

				return TypedResults.InternalServerError(response);
			}
		}
	}

	/// <summary>
	/// Builds an <seealso cref="IResult"/> response for an unsuccessful operation.
	/// </summary>
	///
	/// <param name="httpContext">The http context.</param>
	protected IResult BuildErrorResult<TContract>(HttpContext httpContext, StandardException exception) where TContract : class
	{
		// Get the services
		var localizer = httpContext.RequestServices.GetService<ILocalizer>()!;

		// Build the response
		var response = new StandardResult<TContract>
		{
			Success = false,
			Message = exception.Message,
			StatusCode = exception.GetStatusCode(),
			Data = null,
			Errors = exception.Messages
		};

		// Adjust the message when necessary
		switch (response.StatusCode)
		{
			case StatusCodes.Status400BadRequest:
			{
				response.Message = this.GetValidationErrorMessage(localizer);

				return TypedResults.BadRequest(response);
			}
			case StatusCodes.Status404NotFound:
			{
				return TypedResults.NotFound(response);
			}
			default:
			{
				response.Message = this.GetUnexpectedErrorMessage(localizer);
				response.Errors = [response.Message];

				return TypedResults.InternalServerError(response);
			}
		}
	}

	/// <summary>
	/// Builds an <seealso cref="IResult"/> response for an unsuccessful operation.
	/// </summary>
	///
	/// <param name="validationResult">The validation result.</param>
	protected IResult BuildErrorResult(HttpContext httpContext, ValidationResult validationResult)
	{
		// Get the services
		var localizer = httpContext.RequestServices.GetService<ILocalizer>()!;

		// Build the response
		var response = new StandardResult
		{
			Success = false,
			Message = this.GetValidationErrorMessage(localizer),
			StatusCode = StatusCodes.Status400BadRequest,
			Errors = validationResult.Errors.Select((error) => error.ErrorMessage)
		};

		return TypedResults.BadRequest(response);
	}

	/// <summary>
	/// Builds an <seealso cref="IResult"/> response for an unsuccessful operation.
	/// </summary>
	///
	/// <param name="validationResult">The validation result.</param>
	protected IResult BuildErrorResult<TContract>(HttpContext httpContext, ValidationResult validationResult) where TContract : class
	{
		// Get the services
		var localizer = httpContext.RequestServices.GetService<ILocalizer>()!;

		// Build the response
		var response = new StandardResult<TContract>
		{
			Success = false,
			Message = this.GetValidationErrorMessage(localizer),
			StatusCode = StatusCodes.Status400BadRequest,
			Data = null,
			Errors = validationResult.Errors.Select((error) => error.ErrorMessage)
		};

		return TypedResults.BadRequest(response);
	}
	#endregion

	#region [Methods] Messages
	/// <summary>
	/// Returns the message that is sent when a 'Create' operation is successful.
	/// </summary>
	///
	/// <param name="localizer">The localizer.</param>
	protected virtual string GetCreateSuccessfulMessage(ILocalizer localizer)
	{
		return localizer.GetString(SharedResources.CONTROLLER_CREATE_SUCCESSFUL);
	}

	/// <summary>
	/// Returns the message that is sent when an 'Update' operation is successful.
	/// </summary>
	///
	/// <param name="localizer">The localizer.</param>
	protected virtual string GetUpdateSuccessfulMessage(ILocalizer localizer)
	{
		return localizer.GetString(SharedResources.CONTROLLER_UPDATE_SUCCESSFUL);
	}

	/// <summary>
	/// Returns the message that is sent when a 'Delete' operation is successful.
	/// </summary>
	///
	/// <param name="localizer">The localizer.</param>
	protected virtual string GetDeleteSuccessfulMessage(ILocalizer localizer)
	{
		return localizer.GetString(SharedResources.CONTROLLER_DELETE_SUCCESSFUL);
	}

	/// <summary>
	/// Returns the message that is sent when a 'Get' operation is successful.
	/// </summary>
	///
	/// <param name="localizer">The localizer.</param>
	protected virtual string GetGetSuccessfulMessage(ILocalizer localizer)
	{
		return localizer.GetString(SharedResources.CONTROLLER_GET_SUCCESSFUL);
	}

	/// <summary>
	/// Returns the message that is sent when a 'GetAll' operation is successful.
	/// </summary>
	///
	/// <param name="localizer">The localizer.</param>
	protected virtual string GetGetAllSuccessfulMessage(ILocalizer localizer)
	{
		return localizer.GetString(SharedResources.CONTROLLER_GET_ALL_SUCCESSFUL);
	}

	/// <summary>
	/// Returns the message that is sent when an unexpected error occurs.
	/// </summary>
	///
	/// <param name="localizer">The localizer.</param>
	protected virtual string GetUnexpectedErrorMessage(ILocalizer localizer)
	{
		return localizer.GetString(SharedResources.ERROR_UNEXPECTED);
	}

	/// <summary>
	/// Returns the message that is sent when a validation error occurs.
	/// </summary>
	///
	/// <param name="localizer">The localizer.</param>
	protected virtual string GetValidationErrorMessage(ILocalizer localizer)
	{
		return localizer.GetString(SharedResources.ERROR_VALIDATION);
	}
	#endregion
}
