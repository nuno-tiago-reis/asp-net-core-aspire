namespace Memento.Aspire.Domain.Api.Modules;

using FluentValidation;
using Memento.Aspire.Domain.Api.Constants;
using Memento.Aspire.Domain.Service.Contracts.Book;
using Memento.Aspire.Domain.Service.Messaging.Book.Commands;
using Memento.Aspire.Domain.Service.Messaging.Book.Queries;
using Memento.Aspire.Shared.Api;
using Memento.Aspire.Shared.Cache;
using Memento.Aspire.Shared.Extensions;
using Memento.Aspire.Shared.Messaging;
using Memento.Aspire.Shared.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

/// <summary>
/// Implements the API module for the 'Book' entity that provides the base for the CRUD operations.
/// </summary>
///
/// <seealso cref="EntityModule" />
public sealed class BookModule : EntityModule
{
	#region [Constants]
	/// <summary>
	/// The default route.
	/// </summary>
	private const string BASE_ROUTE = "/api/minimal-apis/books";
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="BookModule"/> class.
	/// </summary>
	public BookModule() : base(BASE_ROUTE)
	{
		// Intentionally Empty.
	}
	#endregion

	#region [Methods] Module
	/// <inheritdoc/>
	public override void AddRoutes(IEndpointRouteBuilder builder)
	{
		builder
			.MapPost($"", this.HandleCreateAsync)
			.Produces<StandardResult<BookDetailContract>>(StatusCodes.Status201Created)
			.Produces<StandardResult<BookDetailContract>>(StatusCodes.Status400BadRequest)
			.Produces<StandardResult<BookDetailContract>>(StatusCodes.Status500InternalServerError)
			.RequireAuthorization()
			.RequireCorrelation()
			.RequireCors()
			.RequireIdempotency()
			.WithOpenApi();

		builder
			.MapPut($"{{id}}", this.HandleUpdateAsync)
			.Produces<StandardResult>(StatusCodes.Status200OK)
			.Produces<StandardResult>(StatusCodes.Status400BadRequest)
			.Produces<StandardResult>(StatusCodes.Status500InternalServerError)
			.RequireAuthorization()
			.RequireCorrelation()
			.RequireCors()
			.RequireIdempotency()
			.WithOpenApi();

		builder
			.MapDelete($"{{id}}", this.HandleDeleteAsync)
			.Produces<StandardResult>(StatusCodes.Status200OK)
			.Produces<StandardResult>(StatusCodes.Status400BadRequest)
			.Produces<StandardResult>(StatusCodes.Status500InternalServerError)
			.RequireAuthorization()
			.RequireCorrelation()
			.RequireCors()
			.RequireIdempotency()
			.WithOpenApi();

		builder
			.MapGet($"{{id}}", this.HandleGetAsync)
			.Produces<StandardResult<BookDetailContract>>(StatusCodes.Status200OK)
			.Produces<StandardResult<BookDetailContract>>(StatusCodes.Status400BadRequest)
			.Produces<StandardResult<BookDetailContract>>(StatusCodes.Status500InternalServerError)
			.RequireAuthorization()
			.RequireCorrelation()
			.RequireCors()
			.WithOpenApi();

		builder
			.MapGet($"", this.HandleGetAllAsync)
			.Produces<StandardResult<Page<BookSummaryContract>>>(StatusCodes.Status200OK)
			.Produces<StandardResult<Page<BookSummaryContract>>>(StatusCodes.Status400BadRequest)
			.Produces<StandardResult<Page<BookSummaryContract>>>(StatusCodes.Status500InternalServerError)
			.RequireAuthorization()
			.RequireCorrelation()
			.RequireCors()
			.WithOpenApi();
	}
	#endregion

	#region [Methods] Handlers
	/// <summary>
	/// Creates an 'Book' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="contract">The contract.</param>
	/// <param name="httpContext">The http context acessor.</param>
	public async Task<IResult> HandleCreateAsync([FromBody] BookFormContract contract, HttpContext httpContext)
	{
		// Get the dependencies
		var messageBus = httpContext.RequestServices.GetService<IMessageBus>()!;
		var cancellationToken = httpContext.RequestAborted;

		// Get the correlation identifier
		var correlationId = httpContext.GetCorrelationId();

		// Validate the parameters
		var validator = httpContext.RequestServices.GetService<IValidator<BookFormContract>>()!;
		var validationResult = await validator.ValidateAsync(contract);

		if (!validationResult.IsValid)
		{
			return this.BuildErrorResult<BookDetailContract>(httpContext, validationResult);
		}

		// Build and execute the command
		var command = new CreateBookCommand
		{
			BookContract = contract,
			CorrelationId = correlationId,
			UserId = httpContext.GetUserId()
		};
		var commandResult = await messageBus.DispatchMessageViaBusAsync<CreateBookCommand, CreateBookCommandResult>(command, cancellationToken);

		if (commandResult.Success)
		{
			return this.BuildCreateResult(httpContext, commandResult.BookContract!);
		}
		else
		{
			return this.BuildErrorResult<BookDetailContract>(httpContext, commandResult.Exception!);
		}
	}

	/// <summary>
	/// Updates an 'Book' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="id">The id.</param>
	/// <param name="contract">The contract.</param>
	/// <param name="httpContext">The http context.</param>
	public async Task<IResult> HandleUpdateAsync([FromRoute] Guid id, [FromBody] BookFormContract contract, HttpContext httpContext)
	{
		// Get the dependencies
		var messageBus = httpContext.RequestServices.GetService<IMessageBus>()!;
		var cancellationToken = httpContext.RequestAborted;

		// Get the correlation identifier
		var correlationId = httpContext.GetCorrelationId();

		// Validate the parameters
		var validator = httpContext.RequestServices.GetService<IValidator<BookFormContract>>()!;
		var validationResult = await validator.ValidateAsync(contract);

		if (!validationResult.IsValid)
		{
			return this.BuildErrorResult(httpContext, validationResult);
		}

		// Build and execute the command
		var command = new UpdateBookCommand
		{
			BookId = id,
			BookContract = contract,
			CorrelationId = correlationId,
			UserId = httpContext.GetUserId()
		};
		var commandResult = await messageBus.DispatchMessageViaBusAsync<UpdateBookCommand, UpdateBookCommandResult>(command, cancellationToken);

		if (commandResult.Success)
		{
			return this.BuildUpdateResult(httpContext);
		}
		else
		{
			return this.BuildErrorResult(httpContext, commandResult.Exception!);
		}
	}

	/// <summary>
	/// Deletes an 'Book' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="id">The id.</param>
	/// <param name="httpContext">The http context.</param>
	public async Task<IResult> HandleDeleteAsync([FromRoute] Guid id, HttpContext httpContext)
	{
		// Get the dependencies
		var messageBus = httpContext.RequestServices.GetService<IMessageBus>()!;
		var cancellationToken = httpContext.RequestAborted;

		// Get the correlation identifier
		var correlationId = httpContext.GetCorrelationId();

		// Build and execute the command
		var command = new DeleteBookCommand
		{
			BookId = id,
			CorrelationId = correlationId,
			UserId = httpContext.GetUserId()
		};
		var commandResult = await messageBus.DispatchMessageViaBusAsync<DeleteBookCommand, DeleteBookCommandResult>(command, cancellationToken);

		if (commandResult.Success)
		{
			return this.BuildDeleteResult(httpContext);
		}
		else
		{
			return this.BuildErrorResult(httpContext, commandResult.Exception!);
		}
	}

	/// <summary>
	/// Gets an 'Book' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="id">The id.</param>
	/// <param name="httpContext">The http context.</param>
	public async Task<IResult> HandleGetAsync([FromRoute] Guid id, HttpContext httpContext)
	{
		// Get the dependencies
		var cache = httpContext.RequestServices.GetService<ICache>()!;
		var messageBus = httpContext.RequestServices.GetService<IMessageBus>()!;
		var cancellationToken = httpContext.RequestAborted;

		// Get the correlation identifier
		var correlationId = httpContext.GetCorrelationId();

		// Try to get the book from the cache
		var cachedBookContract = await cache.TryGetAsync<BookDetailContract>(CacheEntries.GetBookCacheKey(id));

		// If it is there, we short circuit the flow and return straight away
		if (cachedBookContract is not null)
		{
			return this.BuildGetResult(httpContext, cachedBookContract);
		}

		// Build and execute the query
		var query = new GetBookQuery
		{
			BookId = id,
			CorrelationId = correlationId,
			UserId = httpContext.GetUserId()
		};
		var queryResult = await messageBus.DispatchMessageViaBusAsync<GetBookQuery, GetBookQueryResult>(query, cancellationToken);

		if (queryResult.Success)
		{
			return this.BuildGetResult(httpContext, queryResult.BookContract!);
		}
		else
		{
			return this.BuildErrorResult<BookDetailContract>(httpContext, queryResult.Exception!);
		}
	}

	/// <summary>
	/// Gets multiple 'Books' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="filter">The filter.</param>
	/// <param name="httpContext">The http context.</param>
	public async Task<IResult> HandleGetAllAsync([AsParameters] BookFilterContract filter, HttpContext httpContext)
	{
		// Get the dependencies
		var messageBus = httpContext.RequestServices.GetService<IMessageBus>()!;
		var cancellationToken = httpContext.RequestAborted;

		// Get the correlation identifier
		var correlationId = httpContext.GetCorrelationId();

		// Build and execute the query
		var query = new GetBooksQuery
		{
			BookFilterContract = filter,
			CorrelationId = correlationId,
			UserId = httpContext.GetUserId()
		};
		var queryResult = await messageBus.DispatchMessageViaBusAsync<GetBooksQuery, GetBooksQueryResult>(query, cancellationToken);

		if (queryResult.Success)
		{
			return this.BuildGetAllResult(httpContext, queryResult.BookContracts!);
		}
		else
		{
			return this.BuildErrorResult<Page<BookSummaryContract>>(httpContext, queryResult.Exception!);
		}
	}
	#endregion
}
