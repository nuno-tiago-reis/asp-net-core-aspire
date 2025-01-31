namespace Memento.Aspire.Shared.Api;

using FluentValidation.Results;
using Memento.Aspire.Shared.Contracts;
using Memento.Aspire.Shared.Exceptions;
using Memento.Aspire.Shared.Localization;
using Memento.Aspire.Shared.Pagination;
using Memento.Aspire.Shared.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implements an abstract controller that provides the base for the CRUD operations.
/// </summary>
///
/// <seealso cref="ControllerBase" />
public abstract class EntityController : ControllerBase
{
	#region [Properties]
	/// <summary>
	/// The localizer.
	/// </summary>
	protected readonly ILocalizer Localizer;

	/// <summary>
	/// The logger.
	/// </summary>
	protected readonly ILogger Logger;
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="EntityController"/> class.
	/// </summary>
	///
	/// <param name="localizer">The localizer.</param>
	/// <param name="logger">The logger.</param>
	protected EntityController(ILocalizer localizer, ILogger logger)
	{
		this.Localizer = localizer;
		this.Logger = logger;
	}
	#endregion

	#region [Methods] Results
	/// <summary>
	/// Builds an <seealso cref="ActionResult{StandardResult}"/> response for a successful 'Create' operation.
	/// </summary>
	///
	/// <param name="contract">The contract.</param>
	protected ActionResult<StandardResult<TContract>> BuildCreateResult<TContract>(TContract contract) where TContract : EntityContract
	{
		// Build the message
		var message = this.GetCreateSuccessfulMessage();

		// Build the response
		var response = new StandardResult<TContract>
		{
			Success = true,
			Message = message,
			StatusCode = StatusCodes.Status201Created,
			Data = contract,
			Errors = []
		};

		return this.Created(new Uri($"{this.HttpContext.Request.GetDisplayUrl()}/{contract.Id}"), response);
	}

	/// <summary>
	/// Builds an <seealso cref="ActionResult{StandardResult}"/> response for an successful 'Update' operation.
	/// </summary>
	protected ActionResult<StandardResult> BuildUpdateResult()
	{
		// Build the message
		var message = this.GetUpdateSuccessfulMessage();

		// Build the response
		var response = new StandardResult
		{
			Success = true,
			Message = message,
			StatusCode = StatusCodes.Status200OK,
			Errors = []
		};

		return this.Ok(response);
	}

	/// <summary>
	/// Builds an <seealso cref="ActionResult{StandardResult}"/> response for a successful 'Delete' operation.
	/// </summary>
	protected ActionResult<StandardResult> BuildDeleteResult()
	{
		// Build the message
		var message = this.GetDeleteSuccessfulMessage();

		// Build the response
		var response = new StandardResult
		{
			Success = true,
			Message = message,
			StatusCode = StatusCodes.Status200OK,
			Errors = []
		};

		return this.Ok(response);
	}

	/// <summary>
	/// Builds an <seealso cref="ActionResult{StandardResult}"/> response for a successful 'Get' operation.
	/// </summary>
	///
	/// <param name="contract">The contract.</param>
	protected ActionResult<StandardResult<TContract>> BuildGetResult<TContract>(TContract contract) where TContract : EntityContract
	{
		// Build the message
		var message = this.GetGetSuccessfulMessage();

		// Build the response
		var response = new StandardResult<TContract>
		{
			Success = true,
			Message = message,
			StatusCode = StatusCodes.Status200OK,
			Data = contract,
			Errors = []
		};

		return this.Ok(response);
	}

	/// <summary>
	/// Builds an <seealso cref="ActionResult{StandardResult}"/> response for a successful 'GetAll' operation.
	/// </summary>
	///
	/// <param name="contracts">The contracts.</param>
	protected ActionResult<StandardResult<Page<TContract>>> BuildGetAllResult<TContract>(Page<TContract> contracts) where TContract : EntityContract
	{
		// Build the message
		var message = this.GetGetAllSuccessfulMessage();

		// Build the response
		var response = new StandardResult<Page<TContract>>
		{
			Success = true,
			Message = message,
			StatusCode = StatusCodes.Status200OK,
			Data = contracts,
			Errors = []
		};

		return this.Ok(response);
	}

	/// <summary>
	/// Builds an <seealso cref="ActionResult{StandardResult}"/> response for an unsuccessful operation.
	/// </summary>
	///
	/// <param name="exception">The exception.</param>
	protected ActionResult<StandardResult> BuildErrorResult(StandardException exception)
	{
		// Build the response
		var response = new StandardResult
		{
			Success = false,
			Message = exception.Message,
			StatusCode = exception.GetStatusCode(),
			Errors = exception.Messages
		};

		// Adjust the message when necessary
		switch (exception.Type)
		{
			case StandardExceptionType.BadRequest:
			{
				response.Message = this.GetValidationErrorMessage();
				break;
			}
			case StandardExceptionType.NotFound:
			{
				break;
			}
			default:
			{
				response.Message = this.GetUnexpectedErrorMessage();
				response.Errors = [response.Message];
				break;
			}
		}

		return this.StatusCode(response.StatusCode, response);
	}

	/// <summary>
	/// Builds an <seealso cref="ActionResult{StandardResult}"/> response for an unsuccessful operation.
	/// </summary>
	///
	/// <param name="exception">The exception.</param>
	protected ActionResult<StandardResult<TContract>> BuildErrorResult<TContract>(StandardException exception) where TContract : class
	{
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
		switch (exception.Type)
		{
			case StandardExceptionType.BadRequest:
			{
				response.Message = this.GetValidationErrorMessage();
				break;
			}
			case StandardExceptionType.NotFound:
			{
				break;
			}
			default:
			{
				response.Message = this.GetUnexpectedErrorMessage();
				response.Errors = [ response.Message ];
				break;
			}
		}

		return this.StatusCode(response.StatusCode, response);
	}

	/// <summary>
	/// Builds an <seealso cref="ActionResult{StandardResult}"/> response for an unsuccessful operation.
	/// </summary>
	///
	/// <param name="validationResult">The validation result.</param>
	protected ActionResult<StandardResult> BuildErrorResult(ValidationResult validationResult)
	{
		// Build the response
		var response = new StandardResult
		{
			Success = false,
			Message = this.GetValidationErrorMessage(),
			StatusCode = StatusCodes.Status400BadRequest,
			Errors = validationResult.Errors.Select((error) => error.ErrorMessage)
		};

		return this.BadRequest(response);
	}

	/// <summary>
	/// Builds an <seealso cref="ActionResult{StandardResult}"/> response for an unsuccessful operation.
	/// </summary>
	///
	/// <param name="validationResult">The validation result.</param>
	protected ActionResult<StandardResult<TContract>> BuildErrorResult<TContract>(ValidationResult validationResult) where TContract : class
	{
		// Build the response
		var response = new StandardResult<TContract>
		{
			Success = false,
			Message = this.GetValidationErrorMessage(),
			StatusCode = StatusCodes.Status400BadRequest,
			Data = null,
			Errors = validationResult.Errors.Select((error) => error.ErrorMessage)
		};

		return this.BadRequest(response);
	}
	#endregion

	#region [Methods] Messages
	/// <summary>
	/// Returns the message that is sent when a 'Create' operation is successful.
	/// </summary>
	protected virtual string GetCreateSuccessfulMessage()
	{
		return this.Localizer.GetString(SharedResources.CONTROLLER_CREATE_SUCCESSFUL);
	}

	/// <summary>
	/// Returns the message that is sent when an 'Update' operation is successful.
	/// </summary>
	protected virtual string GetUpdateSuccessfulMessage()
	{
		return this.Localizer.GetString(SharedResources.CONTROLLER_UPDATE_SUCCESSFUL);
	}

	/// <summary>
	/// Returns the message that is sent when a 'Delete' operation is successful.
	/// </summary>
	protected virtual string GetDeleteSuccessfulMessage()
	{
		return this.Localizer.GetString(SharedResources.CONTROLLER_DELETE_SUCCESSFUL);
	}

	/// <summary>
	/// Returns the message that is sent when a 'Get' operation is successful.
	/// </summary>
	protected virtual string GetGetSuccessfulMessage()	
	{
		return this.Localizer.GetString(SharedResources.CONTROLLER_GET_SUCCESSFUL);
	}

	/// <summary>
	/// Returns the message that is sent when a 'GetAll' operation is successful.
	/// </summary>
	protected virtual string GetGetAllSuccessfulMessage()
	{
		return this.Localizer.GetString(SharedResources.CONTROLLER_GET_ALL_SUCCESSFUL);
	}

	/// <summary>
	/// Returns the message that is sent when an unexpected error occurs.
	/// </summary>
	protected virtual string GetUnexpectedErrorMessage()
	{
		return this.Localizer.GetString(SharedResources.ERROR_UNEXPECTED);
	}

	/// <summary>
	/// Returns the message that is sent when a validation error occurs.
	/// </summary>
	protected virtual string GetValidationErrorMessage()
	{
		return this.Localizer.GetString(SharedResources.ERROR_VALIDATION);
	}
	#endregion
}
