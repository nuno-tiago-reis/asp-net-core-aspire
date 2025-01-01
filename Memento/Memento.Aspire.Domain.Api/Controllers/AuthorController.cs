namespace Memento.Aspire.Domain.Api.Controllers;

using FluentValidation;
using Memento.Aspire.Domain.Api.Constants;
using Memento.Aspire.Domain.Service.Contracts.Author;
using Memento.Aspire.Domain.Service.Messaging.Author.Commands;
using Memento.Aspire.Domain.Service.Messaging.Author.Queries;
using Memento.Aspire.Shared.Api;
using Memento.Aspire.Shared.Cache;
using Memento.Aspire.Shared.Contracts;
using Memento.Aspire.Shared.Extensions;
using Memento.Aspire.Shared.Localization;
using Memento.Aspire.Shared.Messaging;
using Memento.Aspire.Shared.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Implements the API controller for the 'Author' entity that provides the base for the CRUD operations.
/// </summary>
///
/// <seealso cref="EntityController" />
[ApiController]
[Authorize]
[Route("/api/controllers/[controller]")]
public sealed class AuthorController : EntityController
{
	#region [Properties]
	/// <summary>
	/// The cache.
	/// </summary>
	private readonly ICache Cache;

	/// <summary>
	/// The message bus.
	/// </summary>
	private readonly IMessageBus MessageBus;
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="AuthorController"/> class.
	/// </summary>
	///
	/// <param name="cache">The cache.</param>
	/// <param name="localizer">The localizer.</param>
	/// <param name="logger">The logger.</param>
	/// <param name="messageBus">The message bus.</param>
	public AuthorController(ICache cache, ILocalizer localizer, ILogger<AuthorController> logger, IMessageBus messageBus) : base(localizer, logger)
	{
		this.Cache = cache;
		this.MessageBus = messageBus;
	}
	#endregion

	#region [Methods]
	/// <summary>
	/// Creates an 'Author' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="contract">The contract.</param>
	[HttpPost]
	[ProducesResponseType<StandardResult<AuthorDetailContract>>(StatusCodes.Status201Created)]
	[ProducesResponseType<StandardResult<AuthorDetailContract>>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType<StandardResult<AuthorDetailContract>>(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<StandardResult<AuthorDetailContract>>> CreateAsync([FromBody] AuthorFormContract contract)
	{
		// Validate the parameters
		var validator = this.HttpContext.RequestServices.GetService<IValidator<AuthorFormContract>>()!;
		var validationResult = await validator.ValidateAsync(contract);

		if (!validationResult.IsValid)
		{
			return this.BuildErrorResult<AuthorDetailContract>(validationResult);
		}

		// Build and execute the command
		var command = new CreateAuthorCommand
		{
			AuthorContract = contract,
			CorrelationId = Guid.NewGuid(),
			IdempotencyId = Guid.NewGuid(),
			UserId = this.HttpContext.GetUserId()
		};
		var commandResult = await this.MessageBus.RequestResponseViaBusAsync<CreateAuthorCommand, CreateAuthorCommandResult>(command, this.HttpContext.RequestAborted);

		if (commandResult.Success)
		{
			return this.BuildCreateResult(commandResult.AuthorContract!);
		}
		else
		{
			return this.BuildErrorResult<AuthorDetailContract>(commandResult.Exception!);
		}
	}

	/// <summary>
	/// Updates an 'Author' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="id">The identifier.</param>
	/// <param name="contract">The contract.</param>
	[HttpPut("{id:Guid}")]
	[ProducesResponseType<StandardResult>(StatusCodes.Status200OK)]
	[ProducesResponseType<StandardResult>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType<StandardResult>(StatusCodes.Status404NotFound)]
	[ProducesResponseType<StandardResult>(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<StandardResult>> UpdateAsync([FromRoute] Guid id, [FromBody] AuthorFormContract contract)
	{
		// Validate the parameters
		var validator = this.HttpContext.RequestServices.GetService<IValidator<AuthorFormContract>>()!;
		var validationResult = await validator.ValidateAsync(contract);

		if (!validationResult.IsValid)
		{
			return this.BuildErrorResult(validationResult);
		}

		// Build and execute the command
		var command = new UpdateAuthorCommand
		{
			AuthorId = id,
			AuthorContract = contract,
			CorrelationId = Guid.NewGuid(),
			IdempotencyId = Guid.NewGuid(),
			UserId = this.HttpContext.GetUserId()
		};
		var commandResult = await this.MessageBus.RequestResponseViaBusAsync<UpdateAuthorCommand, UpdateAuthorCommandResult>(command, this.HttpContext.RequestAborted);

		if (commandResult.Success)
		{
			return this.BuildUpdateResult();
		}
		else
		{
			return this.BuildErrorResult(commandResult.Exception!);
		}
	}

	/// <summary>
	/// Deletes an 'Author' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="id">The identifier.</param>
	[HttpDelete("{id:Guid}")]
	[ProducesResponseType<StandardResult>(StatusCodes.Status200OK)]
	[ProducesResponseType<StandardResult>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType<StandardResult>(StatusCodes.Status404NotFound)]
	[ProducesResponseType<StandardResult>(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<StandardResult>> DeleteAsync([FromRoute] Guid id)
	{
		// Build and execute the command
		var command = new DeleteAuthorCommand
		{
			AuthorId = id,
			CorrelationId = Guid.NewGuid(),
			IdempotencyId = Guid.NewGuid(),
			UserId = this.HttpContext.GetUserId()
		};
		var commandResult = await this.MessageBus.RequestResponseViaBusAsync<DeleteAuthorCommand, DeleteAuthorCommandResult>(command, this.HttpContext.RequestAborted);

		if (commandResult.Success)
		{
			return this.BuildDeleteResult();
		}
		else
		{
			return this.BuildErrorResult(commandResult.Exception!);
		}
	}

	/// <summary>
	/// Gets an 'Author' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="id">The identifier.</param>
	[HttpGet("{id:Guid}")]
	[ProducesResponseType<StandardResult<AuthorDetailContract>>(StatusCodes.Status200OK)]
	[ProducesResponseType<StandardResult<AuthorDetailContract>>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType<StandardResult<AuthorDetailContract>>(StatusCodes.Status404NotFound)]
	[ProducesResponseType<StandardResult<AuthorDetailContract>>(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<StandardResult<AuthorDetailContract>>> GetAsync([FromRoute] Guid id)
	{
		// Try to get the author from the cache
		var cachedAuthorContract = await this.Cache.TryGetAsync<AuthorDetailContract>(CacheEntries.GetAuthorCacheKey(id));

		// If it is there, we short circuit the flow and return straight away
		if (cachedAuthorContract is not null)
		{
			return this.BuildGetResult(cachedAuthorContract);
		}

		// Build and execute the query
		var query = new GetAuthorQuery
		{
			AuthorId = id,
			CorrelationId = Guid.NewGuid(),
			UserId = this.HttpContext.GetUserId()
		};
		var queryResult = await this.MessageBus.RequestResponseViaBusAsync<GetAuthorQuery, GetAuthorQueryResult>(query, this.HttpContext.RequestAborted);

		if (queryResult.Success)
		{
			return this.BuildGetResult(queryResult.AuthorContract!);
		}
		else
		{
			return this.BuildErrorResult<AuthorDetailContract>(queryResult.Exception!);
		}
	}

	/// <summary>
	/// Gets multiple 'Authors' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="filter">The filter.</param>
	[HttpGet]
	[ProducesResponseType<StandardResult<Page<AuthorSummaryContract>>>(StatusCodes.Status200OK)]
	[ProducesResponseType<StandardResult<Page<AuthorSummaryContract>>>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType<StandardResult<Page<AuthorSummaryContract>>>(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<StandardResult<Page<AuthorSummaryContract>>>> GetAsync([FromQuery] AuthorFilterContract filter)
	{
		// Build and execute the query
		var query = new GetAuthorsQuery
		{
			AuthorFilterContract = filter,
			CorrelationId = Guid.NewGuid(),
			UserId = this.HttpContext.GetUserId()
		};
		var queryResult = await this.MessageBus.RequestResponseViaBusAsync<GetAuthorsQuery, GetAuthorsQueryResult>(query, this.HttpContext.RequestAborted);

		if (queryResult.Success)
		{
			return this.BuildGetAllResult(queryResult.AuthorContracts!);
		}
		else
		{
			return this.BuildErrorResult<Page<AuthorSummaryContract>>(queryResult.Exception!);
		}
	}
	#endregion
}
