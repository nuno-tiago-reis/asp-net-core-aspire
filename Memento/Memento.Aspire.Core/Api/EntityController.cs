namespace Memento.Aspire.Core.Api;

using FluentValidation.Results;
using Memento.Aspire.Core.Contracts;
using Memento.Aspire.Core.Exceptions;
using Memento.Aspire.Core.Localization;
using Memento.Aspire.Core.Pagination;
using Memento.Aspire.Core.Resources;
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
	protected ILocalizer Localizer { get; init; }

	/// <summary>
	/// The logger.
	/// </summary>
	protected ILogger Logger { get; init; }
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
		// Prepare the response
		var message = this.GetCreateSuccessfulMessage();

		// Build the response
		var response = new StandardResult<TContract>
		{
			Success = true,
			StatusCode = StatusCodes.Status201Created,
			Message = message,
			Errors = [],
			Data = contract
		};

		return this.Created(new Uri($"{this.HttpContext.Request.GetDisplayUrl()}/{contract.Id}"), response);
	}

	/// <summary>
	/// Builds an <seealso cref="ActionResult{StandardResult}"/> response for an successful 'Update' operation.
	/// </summary>
	protected ActionResult<StandardResult> BuildUpdateResult()
	{
		// Prepare the response
		var message = this.GetUpdateSuccessfulMessage();

		// Build the response
		var response = new StandardResult
		{
			Success = true,
			StatusCode = StatusCodes.Status200OK,
			Message = message,
			Errors = []
		};

		return this.Ok(response);
	}

	/// <summary>
	/// Builds an <seealso cref="ActionResult{StandardResult}"/> response for a successful 'Delete' operation.
	/// </summary>
	protected ActionResult<StandardResult> BuildDeleteResult()
	{
		// Prepare the response
		var message = this.GetDeleteSuccessfulMessage();

		// Build the response
		var response = new StandardResult
		{
			Success = true,
			StatusCode = StatusCodes.Status200OK,
			Message = message,
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
		// Prepare the response
		var message = this.GetGetSuccessfulMessage();

		// Build the response
		var response = new StandardResult<TContract>
		{
			Success = true,
			StatusCode = StatusCodes.Status200OK,
			Message = message,
			Errors = [],
			Data = contract
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
		// Prepare the response
		var message = this.GetGetAllSuccessfulMessage();

		// Build the response
		var response = new StandardResult<Page<TContract>>
		{
			Success = true,
			StatusCode = StatusCodes.Status200OK,
			Message = message,
			Errors = [],
			Data = contracts
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
		// Prepare the response
		var statusCode = exception.GetStatusCode();
		var message = exception.Message;
		var errors = exception.Messages;

		// Adjust the response when necessary
		switch (exception.Type)
		{
			case StandardExceptionType.BadRequest:
			{
				message = this.GetValidationErrorMessage();
				break;
			}
			case StandardExceptionType.NotFound:
			{
				break;
			}
			default:
			{
				message = this.GetUnexpectedErrorMessage();
				errors = [message];
				break;
			}
		}

		// Build the response
		var response = new StandardResult
		{
			Success = false,
			StatusCode = statusCode,
			Message = message,
			Errors = errors
		};

		return this.StatusCode(response.StatusCode, response);
	}

	/// <summary>
	/// Builds an <seealso cref="ActionResult{StandardResult}"/> response for an unsuccessful operation.
	/// </summary>
	///
	/// <param name="exception">The exception.</param>
	protected ActionResult<StandardResult<TContract>> BuildErrorResult<TContract>(StandardException exception) where TContract : class
	{
		// Prepare the response
		var statusCode = exception.GetStatusCode();
		var message = exception.Message;
		var errors = exception.Messages;

		// Adjust the response when necessary
		switch (exception.Type)
		{
			case StandardExceptionType.BadRequest:
			{
				message = this.GetValidationErrorMessage();
				break;
			}
			case StandardExceptionType.NotFound:
			{
				break;
			}
			default:
			{
				message = this.GetUnexpectedErrorMessage();
				errors = [message];
				break;
			}
		}

		// Build the response
		var response = new StandardResult<TContract>
		{
			Success = false,
			StatusCode = statusCode,
			Message = message,
			Errors = errors,
			Data = null
		};

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
