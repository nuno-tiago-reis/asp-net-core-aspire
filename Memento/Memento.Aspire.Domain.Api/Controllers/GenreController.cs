namespace Memento.Aspire.Domain.Api.Controllers;

using FluentValidation;
using Memento.Aspire.Domain.Api.Constants;
using Memento.Aspire.Domain.Service.Contracts.Genre;
using Memento.Aspire.Domain.Service.Messaging.Genre.Commands;
using Memento.Aspire.Domain.Service.Messaging.Genre.Queries;
using Memento.Aspire.Shared.Api;
using Memento.Aspire.Shared.Cache;
using Memento.Aspire.Shared.Extensions;
using Memento.Aspire.Shared.Localization;
using Memento.Aspire.Shared.Messaging;
using Memento.Aspire.Shared.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Implements the API controller for the 'Genre' entity that provides the base for the CRUD operations.
/// </summary>
///
/// <seealso cref="EntityController" />
[ApiController]
[Authorize]
[Correlate]
[Route("/api/controllers/[controller]")]
public sealed class GenreController : EntityController
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
	/// Initializes a new instance of the <see cref="GenreController"/> class.
	/// </summary>
	///
	/// <param name="cache">The cache.</param>
	/// <param name="localizer">The localizer.</param>
	/// <param name="logger">The logger.</param>
	/// <param name="messageBus">The message bus.</param>
	public GenreController(ICache cache, ILocalizer localizer, ILogger<GenreController> logger, IMessageBus messageBus) : base(localizer, logger)
	{
		this.Cache = cache;
		this.MessageBus = messageBus;
	}
	#endregion

	#region [Methods]
	/// <summary>
	/// Creates an 'Genre' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="contract">The contract.</param>
	[HttpPost]
	[Idempotent]
	[ProducesResponseType<StandardResult<GenreDetailContract>>(StatusCodes.Status201Created)]
	[ProducesResponseType<StandardResult<GenreDetailContract>>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType<StandardResult<GenreDetailContract>>(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<StandardResult<GenreDetailContract>>> CreateAsync([FromBody] GenreFormContract contract)
	{
		// Get the correlation identifier
		var correlationId = this.HttpContext.GetCorrelationId();

		// Validate the parameters
		var validator = this.HttpContext.RequestServices.GetService<IValidator<GenreFormContract>>()!;
		var validationResult = await validator.ValidateAsync(contract);

		if (!validationResult.IsValid)
		{
			return this.BuildErrorResult<GenreDetailContract>(validationResult);
		}

		// Build and execute the command
		var command = new CreateGenreCommand
		{
			GenreContract = contract,
			CorrelationId = correlationId,
			UserId = this.HttpContext.GetUserId()
		};
		var commandResult = await this.MessageBus.DispatchMessageViaBusAsync<CreateGenreCommand, CreateGenreCommandResult>(command, this.HttpContext.RequestAborted);

		if (commandResult.Success)
		{
			return this.BuildCreateResult(commandResult.GenreContract!);
		}
		else
		{
			return this.BuildErrorResult<GenreDetailContract>(commandResult.Exception!);
		}
	}

	/// <summary>
	/// Updates an 'Genre' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="id">The identifier.</param>
	/// <param name="contract">The contract.</param>
	[HttpPut("{id:Guid}")]
	[Idempotent]
	[ProducesResponseType<StandardResult>(StatusCodes.Status200OK)]
	[ProducesResponseType<StandardResult>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType<StandardResult>(StatusCodes.Status404NotFound)]
	[ProducesResponseType<StandardResult>(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<StandardResult>> UpdateAsync([FromRoute] Guid id, [FromBody] GenreFormContract contract)
	{
		// Get the correlation identifier
		var correlationId = this.HttpContext.GetCorrelationId();

		// Validate the parameters
		var validator = this.HttpContext.RequestServices.GetService<IValidator<GenreFormContract>>()!;
		var validationResult = await validator.ValidateAsync(contract);

		if (!validationResult.IsValid)
		{
			return this.BuildErrorResult(validationResult);
		}

		// Build and execute the command
		var command = new UpdateGenreCommand
		{
			GenreId = id,
			GenreContract = contract,
			CorrelationId = correlationId,
			UserId = this.HttpContext.GetUserId()
		};
		var commandResult = await this.MessageBus.DispatchMessageViaBusAsync<UpdateGenreCommand, UpdateGenreCommandResult>(command, this.HttpContext.RequestAborted);

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
	/// Deletes an 'Genre' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="id">The identifier.</param>
	[HttpDelete("{id:Guid}")]
	[Idempotent]
	[ProducesResponseType<StandardResult>(StatusCodes.Status200OK)]
	[ProducesResponseType<StandardResult>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType<StandardResult>(StatusCodes.Status404NotFound)]
	[ProducesResponseType<StandardResult>(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<StandardResult>> DeleteAsync([FromRoute] Guid id)
	{
		// Get the correlation identifier
		var correlationId = this.HttpContext.GetCorrelationId();

		// Build and execute the command
		var command = new DeleteGenreCommand
		{
			GenreId = id,
			CorrelationId = correlationId,
			UserId = this.HttpContext.GetUserId()
		};
		var commandResult = await this.MessageBus.DispatchMessageViaBusAsync<DeleteGenreCommand, DeleteGenreCommandResult>(command, this.HttpContext.RequestAborted);

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
	/// Gets an 'Genre' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="id">The identifier.</param>
	[HttpGet("{id:Guid}")]
	[ProducesResponseType<StandardResult<GenreDetailContract>>(StatusCodes.Status200OK)]
	[ProducesResponseType<StandardResult<GenreDetailContract>>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType<StandardResult<GenreDetailContract>>(StatusCodes.Status404NotFound)]
	[ProducesResponseType<StandardResult<GenreDetailContract>>(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<StandardResult<GenreDetailContract>>> GetAsync([FromRoute] Guid id)
	{
		// Get the correlation identifier
		var correlationId = this.HttpContext.GetCorrelationId();

		// Try to get the genre from the cache
		var cachedGenreContract = await this.Cache.TryGetAsync<GenreDetailContract>(CacheEntries.GetGenreCacheKey(id));

		// If it is there, we short circuit the flow and return straight away
		if (cachedGenreContract is not null)
		{
			return this.BuildGetResult(cachedGenreContract);
		}

		// Build and execute the query
		var query = new GetGenreQuery
		{
			GenreId = id,
			CorrelationId = correlationId,
			UserId = this.HttpContext.GetUserId()
		};
		var queryResult = await this.MessageBus.DispatchMessageViaBusAsync<GetGenreQuery, GetGenreQueryResult>(query, this.HttpContext.RequestAborted);

		if (queryResult.Success)
		{
			return this.BuildGetResult(queryResult.GenreContract!);
		}
		else
		{
			return this.BuildErrorResult<GenreDetailContract>(queryResult.Exception!);
		}
	}

	/// <summary>
	/// Gets multiple 'Genres' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="filter">The filter.</param>
	[HttpGet]
	[ProducesResponseType<StandardResult<Page<GenreSummaryContract>>>(StatusCodes.Status200OK)]
	[ProducesResponseType<StandardResult<Page<GenreSummaryContract>>>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType<StandardResult<Page<GenreSummaryContract>>>(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<StandardResult<Page<GenreSummaryContract>>>> GetAsync([FromQuery] GenreFilterContract filter)
	{
		// Get the correlation identifier
		var correlationId = this.HttpContext.GetCorrelationId();

		// Build and execute the query
		var query = new GetGenresQuery
		{
			GenreFilterContract = filter,
			CorrelationId = correlationId,
			UserId = this.HttpContext.GetUserId()
		};
		var queryResult = await this.MessageBus.DispatchMessageViaBusAsync<GetGenresQuery, GetGenresQueryResult>(query, this.HttpContext.RequestAborted);

		if (queryResult.Success)
		{
			return this.BuildGetAllResult(queryResult.GenreContracts!);
		}
		else
		{
			return this.BuildErrorResult<Page<GenreSummaryContract>>(queryResult.Exception!);
		}
	}
	#endregion
}
