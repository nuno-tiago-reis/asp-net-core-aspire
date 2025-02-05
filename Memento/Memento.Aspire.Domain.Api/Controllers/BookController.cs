namespace Memento.Aspire.Domain.Api.Controllers;

using FluentValidation;
using Memento.Aspire.Domain.Api.Constants;
using Memento.Aspire.Domain.Service.Contracts.Book;
using Memento.Aspire.Domain.Service.Messaging.Book.Commands;
using Memento.Aspire.Domain.Service.Messaging.Book.Queries;
using Memento.Aspire.Shared.Api;
using Memento.Aspire.Shared.Cache;
using Memento.Aspire.Shared.Extensions;
using Memento.Aspire.Shared.Localization;
using Memento.Aspire.Shared.Messaging;
using Memento.Aspire.Shared.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Implements the API controller for the 'Book' entity that provides the base for the CRUD operations.
/// </summary>
///
/// <seealso cref="EntityController" />
[ApiController]
[Authorize]
[Correlate]
[Route("/api/controllers/[controller]")]
public sealed class BookController : EntityController
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
	/// Initializes a new instance of the <see cref="BookController"/> class.
	/// </summary>
	///
	/// <param name="cache">The cache.</param>
	/// <param name="localizer">The localizer.</param>
	/// <param name="logger">The logger.</param>
	/// <param name="messageBus">The message bus.</param>
	public BookController(ICache cache, ILocalizer localizer, ILogger<BookController> logger, IMessageBus messageBus) : base(localizer, logger)
	{
		this.Cache = cache;
		this.MessageBus = messageBus;
	}
	#endregion

	#region [Methods]
	/// <summary>
	/// Creates an 'Book' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="contract">The contract.</param>
	[HttpPost]
	[Idempotent]
	[ProducesResponseType<StandardResult<BookDetailContract>>(StatusCodes.Status201Created)]
	[ProducesResponseType<StandardResult<BookDetailContract>>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType<StandardResult<BookDetailContract>>(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<StandardResult<BookDetailContract>>> CreateAsync([FromBody] BookFormContract contract)
	{
		// Get the correlation identifier
		var correlationId = this.HttpContext.GetCorrelationId();

		// Validate the parameters
		var validator = this.HttpContext.RequestServices.GetService<IValidator<BookFormContract>>()!;
		var validationResult = await validator.ValidateAsync(contract);

		if (!validationResult.IsValid)
		{
			return this.BuildErrorResult<BookDetailContract>(validationResult);
		}

		// Build and execute the command
		var command = new CreateBookCommand
		{
			BookContract = contract,
			CorrelationId = correlationId,
			UserId = this.HttpContext.GetUserId()
		};
		var commandResult = await this.MessageBus.RequestResponseViaBusAsync<CreateBookCommand, CreateBookCommandResult>(command, this.HttpContext.RequestAborted);

		if (commandResult.Success)
		{
			return this.BuildCreateResult(commandResult.BookContract!);
		}
		else
		{
			return this.BuildErrorResult<BookDetailContract>(commandResult.Exception!);
		}
	}

	/// <summary>
	/// Updates an 'Book' by sending a message to the underlying service.
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
	public async Task<ActionResult<StandardResult>> UpdateAsync([FromRoute] Guid id, [FromBody] BookFormContract contract)
	{
		// Get the correlation identifier
		var correlationId = this.HttpContext.GetCorrelationId();

		// Validate the parameters
		var validator = this.HttpContext.RequestServices.GetService<IValidator<BookFormContract>>()!;
		var validationResult = await validator.ValidateAsync(contract);

		if (!validationResult.IsValid)
		{
			return this.BuildErrorResult(validationResult);
		}

		// Build and execute the command
		var command = new UpdateBookCommand
		{
			BookId = id,
			BookContract = contract,
			CorrelationId = correlationId,
			UserId = this.HttpContext.GetUserId()
		};
		var commandResult = await this.MessageBus.RequestResponseViaBusAsync<UpdateBookCommand, UpdateBookCommandResult>(command, this.HttpContext.RequestAborted);

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
	/// Deletes an 'Book' by sending a message to the underlying service.
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
		var command = new DeleteBookCommand
		{
			BookId = id,
			CorrelationId = correlationId,
			UserId = this.HttpContext.GetUserId()
		};
		var commandResult = await this.MessageBus.RequestResponseViaBusAsync<DeleteBookCommand, DeleteBookCommandResult>(command, this.HttpContext.RequestAborted);

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
	/// Gets an 'Book' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="id">The identifier.</param>
	[HttpGet("{id:Guid}")]
	[ProducesResponseType<StandardResult<BookDetailContract>>(StatusCodes.Status200OK)]
	[ProducesResponseType<StandardResult<BookDetailContract>>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType<StandardResult<BookDetailContract>>(StatusCodes.Status404NotFound)]
	[ProducesResponseType<StandardResult<BookDetailContract>>(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<StandardResult<BookDetailContract>>> GetAsync([FromRoute] Guid id)
	{
		// Get the correlation identifier
		var correlationId = this.HttpContext.GetCorrelationId();

		// Try to get the book from the cache
		var cachedBookContract = await this.Cache.TryGetAsync<BookDetailContract>(CacheEntries.GetBookCacheKey(id));

		// If it is there, we short circuit the flow and return straight away
		if (cachedBookContract is not null)
		{
			return this.BuildGetResult(cachedBookContract);
		}

		// Build and execute the query
		var query = new GetBookQuery
		{
			BookId = id,
			CorrelationId = correlationId,
			UserId = this.HttpContext.GetUserId()
		};
		var queryResult = await this.MessageBus.RequestResponseViaBusAsync<GetBookQuery, GetBookQueryResult>(query, this.HttpContext.RequestAborted);

		if (queryResult.Success)
		{
			return this.BuildGetResult(queryResult.BookContract!);
		}
		else
		{
			return this.BuildErrorResult<BookDetailContract>(queryResult.Exception!);
		}
	}

	/// <summary>
	/// Gets multiple 'Books' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="filter">The filter.</param>
	[HttpGet]
	[ProducesResponseType<StandardResult<Page<BookSummaryContract>>>(StatusCodes.Status200OK)]
	[ProducesResponseType<StandardResult<Page<BookSummaryContract>>>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType<StandardResult<Page<BookSummaryContract>>>(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<StandardResult<Page<BookSummaryContract>>>> GetAsync([FromQuery] BookFilterContract filter)
	{
		// Get the correlation identifier
		var correlationId = this.HttpContext.GetCorrelationId();

		// Build and execute the query
		var query = new GetBooksQuery
		{
			BookFilterContract = filter,
			CorrelationId = correlationId,
			UserId = this.HttpContext.GetUserId()
		};
		var queryResult = await this.MessageBus.RequestResponseViaBusAsync<GetBooksQuery, GetBooksQueryResult>(query, this.HttpContext.RequestAborted);

		if (queryResult.Success)
		{
			return this.BuildGetAllResult(queryResult.BookContracts!);
		}
		else
		{
			return this.BuildErrorResult<Page<BookSummaryContract>>(queryResult.Exception!);
		}
	}
	#endregion
}
