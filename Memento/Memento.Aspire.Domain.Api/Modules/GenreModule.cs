namespace Memento.Aspire.Domain.Api.Modules;

using FluentValidation;
using Memento.Aspire.Domain.Api.Constants;
using Memento.Aspire.Domain.Service.Contracts.Genre;
using Memento.Aspire.Domain.Service.Messaging.Genre.Commands;
using Memento.Aspire.Domain.Service.Messaging.Genre.Queries;
using Memento.Aspire.Shared.Api;
using Memento.Aspire.Shared.Cache;
using Memento.Aspire.Shared.Contracts;
using Memento.Aspire.Shared.Extensions;
using Memento.Aspire.Shared.Messaging;
using Memento.Aspire.Shared.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

/// <summary>
/// Implements the API module for the 'Genre' entity that provides the base for the CRUD operations.
/// </summary>
///
/// <seealso cref="EntityModule" />
public sealed class GenreModule : EntityModule
{
	#region [Constants]
	/// <summary>
	/// The default route.
	/// </summary>
	private const string BASE_ROUTE = "/api/minimal-apis/genres";
	#endregion

	#region [Constructors]
	/// <summary>
	/// Initializes a new instance of the <see cref="GenreModule"/> class.
	/// </summary>
	public GenreModule() : base(BASE_ROUTE)
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
			.Produces<StandardResult<GenreDetailContract>>(StatusCodes.Status201Created)
			.Produces<StandardResult<GenreDetailContract>>(StatusCodes.Status400BadRequest)
			.Produces<StandardResult<GenreDetailContract>>(StatusCodes.Status500InternalServerError)
			.RequireCors()
			.RequireAuthorization()
			.WithOpenApi();

		builder
			.MapPut($"{{id}}", this.HandleUpdateAsync)
			.Produces<StandardResult>(StatusCodes.Status200OK)
			.Produces<StandardResult>(StatusCodes.Status400BadRequest)
			.Produces<StandardResult>(StatusCodes.Status500InternalServerError)
			.RequireCors()
			.RequireAuthorization()
			.WithOpenApi();

		builder
			.MapDelete($"{{id}}", this.HandleDeleteAsync)
			.Produces<StandardResult>(StatusCodes.Status200OK)
			.Produces<StandardResult>(StatusCodes.Status400BadRequest)
			.Produces<StandardResult>(StatusCodes.Status500InternalServerError)
			.RequireCors()
			.RequireAuthorization()
			.WithOpenApi();

		builder
			.MapGet($"{{id}}", this.HandleGetAsync)
			.Produces<StandardResult<GenreDetailContract>>(StatusCodes.Status200OK)
			.Produces<StandardResult<GenreDetailContract>>(StatusCodes.Status400BadRequest)
			.Produces<StandardResult<GenreDetailContract>>(StatusCodes.Status500InternalServerError)
			.RequireCors()
			.RequireAuthorization()
			.WithOpenApi();

		builder
			.MapGet($"", this.HandleGetAllAsync)
			.Produces<StandardResult<Page<GenreSummaryContract>>>(StatusCodes.Status200OK)
			.Produces<StandardResult<Page<GenreSummaryContract>>>(StatusCodes.Status400BadRequest)
			.Produces<StandardResult<Page<GenreSummaryContract>>>(StatusCodes.Status500InternalServerError)
			.RequireCors()
			.RequireAuthorization()
			.WithOpenApi();
	}
	#endregion

	#region [Methods] Handlers
	/// <summary>
	/// Creates an 'Genre' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="contract">The contract.</param>
	/// <param name="httpContext">The http context.</param>
	public async Task<IResult> HandleCreateAsync([FromBody] GenreFormContract contract, HttpContext httpContext)
	{
		// Get the dependencies
		var messageBus = httpContext.RequestServices.GetService<IMessageBus>()!;
		var cancellationToken = httpContext.RequestAborted;

		// Validate the parameters
		var validator = httpContext.RequestServices.GetService<IValidator<GenreFormContract>>()!;
		var validationResult = await validator.ValidateAsync(contract);

		if (!validationResult.IsValid)
		{
			return this.BuildErrorResult<GenreDetailContract>(httpContext, validationResult);
		}

		// Build and execute the command
		var command = new CreateGenreCommand
		{
			GenreContract = contract,
			CorrelationId = Guid.NewGuid(),
			IdempotencyId = Guid.NewGuid(),
			UserId = httpContext.GetUserId()
		};
		var commandResult = await messageBus.RequestResponseViaBusAsync<CreateGenreCommand, CreateGenreCommandResult>(command, cancellationToken);

		if (commandResult.Success)
		{
			return this.BuildCreateResult(httpContext, commandResult.GenreContract!);
		}
		else
		{
			return this.BuildErrorResult<GenreDetailContract>(httpContext, commandResult.Exception!);
		}
	}

	/// <summary>
	/// Updates an 'Genre' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="id">The id.</param>
	/// <param name="contract">The contract.</param>
	/// <param name="httpContext">The http context.</param>
	public async Task<IResult> HandleUpdateAsync([FromRoute] Guid id, [FromBody] GenreFormContract contract, HttpContext httpContext)
	{
		// Get the dependencies
		var messageBus = httpContext.RequestServices.GetService<IMessageBus>()!;
		var cancellationToken = httpContext.RequestAborted;

		// Validate the parameters
		var validator = httpContext.RequestServices.GetService<IValidator<GenreFormContract>>()!;
		var validationResult = await validator.ValidateAsync(contract);

		if (!validationResult.IsValid)
		{
			return this.BuildErrorResult(httpContext, validationResult);
		}

		// Build and execute the command
		var command = new UpdateGenreCommand
		{
			GenreId = id,
			GenreContract = contract,
			CorrelationId = Guid.NewGuid(),
			IdempotencyId = Guid.NewGuid(),
			UserId = httpContext.GetUserId()
		};
		var commandResult = await messageBus.RequestResponseViaBusAsync<UpdateGenreCommand, UpdateGenreCommandResult>(command, cancellationToken);

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
	/// Deletes an 'Genre' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="id">The id.</param>
	/// <param name="httpContext">The http context.</param>
	public async Task<IResult> HandleDeleteAsync([FromRoute] Guid id, HttpContext httpContext)
	{
		// Get the dependencies
		var messageBus = httpContext.RequestServices.GetService<IMessageBus>()!;
		var cancellationToken = httpContext.RequestAborted;

		// Build and execute the command
		var command = new DeleteGenreCommand
		{
			GenreId = id,
			CorrelationId = Guid.NewGuid(),
			IdempotencyId = Guid.NewGuid(),
			UserId = httpContext.GetUserId()
		};
		var commandResult = await messageBus.RequestResponseViaBusAsync<DeleteGenreCommand, DeleteGenreCommandResult>(command, cancellationToken);

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
	/// Gets an 'Genre' by sending a message to the underlying service.
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

		// Try to get the genre from the cache
		var cachedGenreContract = await cache.TryGetAsync<GenreDetailContract>(CacheEntries.GetGenreCacheKey(id));

		// If it is there, we short circuit the flow and return straight away
		if (cachedGenreContract is not null)
		{
			return this.BuildGetResult(httpContext, cachedGenreContract);
		}

		// Build and execute the query
		var query = new GetGenreQuery
		{
			GenreId = id,
			CorrelationId = Guid.NewGuid(),
			UserId = httpContext.GetUserId()
		};
		var queryResult = await messageBus.RequestResponseViaBusAsync<GetGenreQuery, GetGenreQueryResult>(query, cancellationToken);

		if (queryResult.Success)
		{
			return this.BuildGetResult(httpContext, queryResult.GenreContract!);
		}
		else
		{
			return this.BuildErrorResult<GenreDetailContract>(httpContext, queryResult.Exception!);
		}
	}

	/// <summary>
	/// Gets multiple 'Genres' by sending a message to the underlying service.
	/// </summary>
	///
	/// <param name="filter">The filter.</param>
	/// <param name="httpContext">The http context.</param>
	public async Task<IResult> HandleGetAllAsync([AsParameters] GenreFilterContract filter, HttpContext httpContext)
	{
		// Get the dependencies
		var messageBus = httpContext.RequestServices.GetService<IMessageBus>()!;
		var cancellationToken = httpContext.RequestAborted;

		// Build and execute the query
		var query = new GetGenresQuery
		{
			GenreFilterContract = filter,
			CorrelationId = Guid.NewGuid(),
			UserId = httpContext.GetUserId()
		};
		var queryResult = await messageBus.RequestResponseViaBusAsync<GetGenresQuery, GetGenresQueryResult>(query, cancellationToken);

		if (queryResult.Success)
		{
			return this.BuildGetAllResult(httpContext, queryResult.GenreContracts!);
		}
		else
		{
			return this.BuildErrorResult<Page<GenreSummaryContract>>(httpContext, queryResult.Exception!);
		}
	}
	#endregion
}
